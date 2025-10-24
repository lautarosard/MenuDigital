// En: JS/Pages/MisOrdenes.js

import { getOrders, updateOrderItems } from "./../APIs/OrderApi.js";import { renderOrderCard, statusConfig, formatLocalDate } from "./../Components/Orders/renderOrderCard.js";

import { renderOrderItemDetail } from "./../Components/Orders/renderOrderItemDetail.js";


// Referencias a los contenedores del HTML
const activeOrdersContainer = document.getElementById('active-orders-container');
const historyOrdersContainer = document.getElementById('history-orders-container');

const modalElement = document.getElementById('orderDetailModal');
const modalTitle = document.getElementById('orderDetailModalTitle'); // <-- ESTA ES LA LÍNEA QUE TE FALTA
const itemsContainer = document.getElementById('order-items-container');
const detailsContainer = document.getElementById('order-summary-details');
// Hacemos 'allOrders' accesible para los listeners
let allOrders = [];
let orderModalInstance = null;
let currentEditingOrderNumber = null;

async function handleSaveChanges() {
    if (!currentEditingOrderNumber) {
        console.error("No hay un ID de orden para actualizar.");
        return;
    }
    
    const btnGuardar = document.getElementById('btn-guardar-cambios');
    btnGuardar.disabled = true; // Deshabilitamos mientras guarda
    btnGuardar.textContent = 'Guardando...';

    try {
        // 1. Encontrar todos los elementos de ítem en el modal
        const itemElements = document.querySelectorAll('#order-items-container .item-card-container');
        
        // 2. Crear el array de ítems actualizados (esto es lo que espera la API)
        const updatedItems = Array.from(itemElements).map(itemEl => {
            // Leemos los valores de los inputs dentro de este ítem
            const quantityInput = itemEl.querySelector('.item-quantity-input');
            const notesInput = itemEl.querySelector('.item-notes-input');
            
            return {
                id: parseInt(itemEl.dataset.itemId), // El ID del item (ej: 7, 8, 9)
                quantity: parseInt(quantityInput.value) || 1, // La nueva cantidad
                notes: notesInput.value || "" // Las nuevas notas
            };
        });

        // 3. Volvemos a crear el objeto que enviaremos a la API
        const updateRequest = {
            items: updatedItems
        };

        // 4. Llamamos a la API enviando 'updateRequest'
        console.log("Enviando actualización:", currentEditingOrderNumber, updateRequest); // <-- MODIFICADO
        await updateOrderItems(currentEditingOrderNumber, updateRequest); // <-- MODIFICADO
        // ===============================================

        alert('¡Orden actualizada con éxito!');
        
        await inicializarMisOrdenes(true);

    } catch (error) {
        console.error("Error al guardar los cambios:", error);
        alert('Hubo un error al guardar. Revisa la consola.');
    } finally {
        // 6. Reactivar el botón y cerrar el modal
        btnGuardar.disabled = false;
        btnGuardar.textContent = 'Guardar Cambios';
        orderModalInstance.hide();
    }
}

function renderOrders(container, orders, emptyMessage) {
    // 1. Limpiamos el contenedor (quitamos el "Buscando...")
    container.innerHTML = '';

    // 2. Si no hay pedidos, mostramos el mensaje
    if (orders.length === 0) {
        container.innerHTML = `<p class="text-center text-muted">${emptyMessage}</p>`;
        return;
    }

    // 3. Creamos un fragmento para mejor rendimiento
    const fragment = document.createDocumentFragment();
    orders.forEach(order => {
        const orderCard = renderOrderCard(order); // Usamos el componente
        fragment.appendChild(orderCard);
    });

    // 4. Añadimos todos los pedidos al DOM de una sola vez
    container.appendChild(fragment);
}

// === INICIO: NUEVA FUNCIÓN PARA POBLAR EL MODAL ===
// ===============================================

function populateOrderModal(order) {
    // 1. Rellenar Título
    modalTitle.textContent = `Detalles de la Orden #${order.orderNumber}`;

    // 2. Rellenar Ítems
    itemsContainer.innerHTML = ''; // Limpiamos ítems anteriores
    if (order.items && order.items.length > 0) {
        order.items.forEach(item => {
            const itemElement = renderOrderItemDetail(item);
            itemsContainer.appendChild(itemElement);
        });
    } else {
        itemsContainer.innerHTML = '<p class="text-muted text-center">Esta orden no tiene ítems.</p>';
    }

    // 3. Rellenar Detalles del Pedido (al final)
    
    // Obtenemos los valores formateados (reutilizando la lógica)
    const statusKey = (order.status && order.status.name) ? order.status.name.toUpperCase() : 'DEFAULT';
    const config = statusConfig[statusKey] || statusConfig['DEFAULT'];
    
    const totalValue = (typeof order.totalAmount === 'number') ? order.totalAmount : 0;
    const totalFormatted = totalValue.toLocaleString('es-AR', {
        style: 'currency',
        currency: 'ARS'
    });

    const deliveryType = order.deliveryType ? order.deliveryType.name : 'No especificado';
    const deliveryTo = order.deliveryTo || 'No especificado';
    
    // Creamos el HTML para los detalles
    detailsContainer.innerHTML = `
        <div class="col-md-6">
            <p><strong>Total:</strong> ${totalFormatted}</p>
            <p><strong>Estado:</strong> <span class="badge bg-${config.color}">${config.text}</span></p>
        </div>
        <div class="col-md-6">
            <p><strong>Tipo de Entrega:</strong> ${deliveryType}</p>
            <p><strong>Nombre/Mesa/Dirección:</strong> ${deliveryTo}</p>
        </div>
    `;
}
/**
 * Función principal para inicializar la página "Mis Órdenes"
 */
async function inicializarMisOrdenes(isRefresh = false) {
    console.log("Inicializando página de 'Mis Órdenes'...");
    if (!isRefresh) {
        console.log("Inicializando listeners y modal por primera vez...");
        
        // Inicializamos la instancia del modal
        orderModalInstance = new bootstrap.Modal(document.getElementById('orderDetailModal'));
        
        // Listener para el botón GUARDAR CAMBIOS
        document.getElementById('btn-guardar-cambios').addEventListener('click', handleSaveChanges);

        // Listener para los botones VER DETALLE (manejador de clics)
        const handleDetailClick = (event) => {
            const detailButton = event.target.closest('.btn-ver-detalle');
            if (!detailButton) return; // No se hizo clic en el botón

            // Obtenemos el ID del botón Y LO CONVERTIMOS A NÚMERO
            const orderNum = parseInt(detailButton.dataset.orderNumber, 10);
            console.log("ID del botón:", orderNum);
            
            // Buscamos la orden completa usando comparación ESTRICTA (===)
            const selectedOrder = allOrders.find(order => order.orderNumber === orderNum);
                
            if (selectedOrder) {
                // Guardamos el ID de la orden que estamos abriendo
                currentEditingOrderNumber = selectedOrder.orderNumber; 
                // Si la encontramos, poblamos el modal
                populateOrderModal(selectedOrder);
                // Y lo mostramos
                orderModalInstance.show();
            } else {
                alert('Error: No se pudieron encontrar los detalles del pedido.');
            }
        };

        // Asignamos el listener a AMBOS contenedores
        activeOrdersContainer.addEventListener('click', handleDetailClick);
        historyOrdersContainer.addEventListener('click', handleDetailClick);
    }
    try {
        // 1. Llamamos a la API para obtener TODAS las órdenes
        // (Asumimos que getOrders() ya está importado)
        allOrders = await getOrders();

        // 2. Filtramos las órdenes
        // === ¡Importante! Hacemos suposiciones sobre los estados ===
        // Ajusta estos arrays si tus estados se llaman diferente
        const activeStates = ['PENDING', 'READY'];
        const historyStates = ['DELIVERED', 'CANCELLED'];

        const activeOrders = allOrders.filter(order => 
            order.status && // 1. Comprueba que 'status' no sea null
            order.status.name && // 2. Comprueba que 'status.name' exista
            activeStates.includes(order.status.name.toUpperCase()) // 3. Compara usando el nombre
        );
        
        const historyOrders = allOrders.filter(order => 
            order.status && 
            order.status.name &&
            historyStates.includes(order.status.name.toUpperCase())
        );
        // 3. "Dibujamos" las órdenes en sus contenedores
        renderOrders(
            activeOrdersContainer, 
            activeOrders, 
            "No tienes pedidos en curso."
        );
        
        renderOrders(
            historyOrdersContainer, 
            historyOrders, 
            "No tienes pedidos en tu historial."
        );

        // Creamos una función manejadora para reutilizarla
        const handleDetailClick = (event) => {
            const detailButton = event.target.closest('.btn-ver-detalle');
            if (!detailButton) return; // No se hizo clic en el botón

        // Obtenemos el ID del botón Y LO CONVERTIMOS A NÚMERO
        const orderNum = parseInt(detailButton.dataset.orderNumber, 10);
        console.log("ID del botón:", orderNum);
        
        // Buscamos la orden completa usando comparación ESTRICTA (===)
        const selectedOrder = allOrders.find(order => order.orderNumber === orderNum);
            
            if (selectedOrder) {
                // Si la encontramos, poblamos el modal
                populateOrderModal(selectedOrder);
                // Y lo mostramos
                orderModalInstance.show();
            } else {
                alert('Error: No se pudieron encontrar los detalles del pedido.');
            }
        };

        // Asignamos el listener a AMBOS contenedores (delegación de eventos)
        activeOrdersContainer.addEventListener('click', handleDetailClick);
        historyOrdersContainer.addEventListener('click', handleDetailClick);


    } catch (error) {
        console.error("Error al cargar las órdenes:", error);
        activeOrdersContainer.innerHTML = '<p class="text-center text-danger">Error al cargar los pedidos. Intente de nuevo.</p>';
        historyOrdersContainer.innerHTML = '';
    }
}

// Exportamos la función para que main.js la pueda llamar
export default inicializarMisOrdenes;