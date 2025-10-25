// En: JS/Pages/MisOrdenes.js

import { getOrders, updateOrderItems } from "./../APIs/OrderApi.js";import { renderOrderCard, statusConfig, formatLocalDate } from "./../Components/Orders/renderOrderCard.js";
import { renderOrderItemDetail } from "./../Components/Orders/renderOrderItemDetail.js";
import { getDishes } from "./../APIs/DishApi.js";
import { renderAddDishItem } from "./../Components/Orders/renderAddDishItem.js";

// Referencias a los contenedores del HTML
const activeOrdersContainer = document.getElementById('active-orders-container');
const historyOrdersContainer = document.getElementById('history-orders-container');

const modalElement = document.getElementById('orderDetailModal');
const modalTitle = document.getElementById('orderDetailModalTitle'); // <-- ESTA ES LA LÍNEA QUE TE FALTA
const itemsContainer = document.getElementById('order-items-container');
const detailsContainer = document.getElementById('order-summary-details');

const addDishModalElement = document.getElementById('addDishModal');
const addDishSearchInput = document.getElementById('add-dish-search-input');
const addDishListContainer = document.getElementById('add-dish-list-container');
let addDishModalInstance = null; // Instancia del modal de Bootstrap
// Hacemos 'allOrders' accesible para los listeners
let allOrders = [];
let orderModalInstance = null;
let currentEditingOrderNumber = null;

async function loadDishesForAdding(searchTerm = '') {
    addDishListContainer.innerHTML = '<p class="text-center text-muted">Buscando...</p>';
    
    const dishes = await getDishes(searchTerm);
    
    addDishListContainer.innerHTML = ''; // Limpiamos
    
    if (dishes.length === 0) {
        addDishListContainer.innerHTML = '<p class="text-center text-muted">No se encontraron platos.</p>';
        return;
    }
    
    dishes.forEach(dish => {
        const dishElement = renderAddDishItem(dish);
        addDishListContainer.appendChild(dishElement);
    });
}
function handleAddDishClick(event) {
    event.preventDefault(); // Evita que el '#' del link recargue la pág.
    
    const dishElement = event.target.closest('.btn-add-dish-to-order');
    if (!dishElement) return;

    // 1. Creamos un objeto 'item' simulado
    const newItem = {
        dish: {
            id: dishElement.dataset.dishId,
            name: dishElement.dataset.dishName
        },
        quantity: 1, // Por defecto empieza en 1
        notes: '',
        id: null // No tiene 'item.id' porque aún no existe en la BD
    };

    // 2. Lo añadimos visualmente al modal de detalles
    // (Usamos el 'itemsContainer' del modal principal)
    const itemElement = renderOrderItemDetail(newItem);
    itemsContainer.appendChild(itemElement);

    // 3. Cerramos el modal de búsqueda
    addDishModalInstance.hide();
}
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
            const parsedQuantity = parseInt(quantityInput.value);
            const finalQuantity = isNaN(parsedQuantity) ? 0 : parsedQuantity;
            return {
                id: itemEl.dataset.dishId, // El ID del item (ej: 7, 8, 9)
                quantity: finalQuantity, // La nueva cantidad
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

function normalizeStatusName(name) {
    if (!name) return '';
    return name.toString().toUpperCase().trim();
}

// Helper: obtener configuración (color/text) a partir del estado
function getStatusConfig(status) {
    // status puede ser { id, name } o string
    const rawName = (typeof status === 'string') ? status : (status && status.name) ? status.name : '';
    const key = normalizeStatusName(rawName);

    // Intentamos buscar en statusConfig con la clave normalizada
    const cfg = statusConfig[key];
    if (cfg) return cfg;

    // Si no hay configuración, devolvemos un fallback legible
    const fallbackText = rawName || 'DESCONOCIDO';
    return { text: fallbackText, color: 'secondary' }; // color 'secondary' para fallback gris
}
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
    
    const statusKey = (order.status && order.status.name) ? order.status.name.toUpperCase() : 'DEFAULT';
    // Luego, usamos 'statusKey' para obtener 'config'
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

        // Listener para los clics DENTRO del modal de detalles
        modalElement.addEventListener('click', (event) => {
            const target = event.target;
            
            // 1. Encontrar el input de cantidad más cercano
            const inputGroup = target.closest('.input-group');
            if (!inputGroup) return; // No se hizo clic en un grupo de botones

            const quantityInput = inputGroup.querySelector('.item-quantity-input');
            if (!quantityInput) return; // No hay input de cantidad

            let currentValue = parseInt(quantityInput.value, 10);

            // 2. Comprobar si se hizo clic en '+' o '-'
            if (target.classList.contains('btn-item-increase')) {
                currentValue++;
                quantityInput.value = currentValue;
            }

            if (target.classList.contains('btn-item-decrease')) {
                if (currentValue > 0) { // No permitimos bajar de 0
                    currentValue--;
                    quantityInput.value = currentValue;
                }
            }
        });
        // 1. Inicializamos la instancia del modal de búsqueda
        addDishModalInstance = new bootstrap.Modal(addDishModalElement);

        // 2. Listener para el botón "Agregar Platos" (del modal principal)
        const btnAgregarPlatos = document.querySelector('#orderDetailModal .btn-success');
        if (btnAgregarPlatos) {
            btnAgregarPlatos.addEventListener('click', () => {
                // Mostramos el modal de búsqueda
                addDishModalInstance.show();
                // Cargamos la lista inicial de platos
                loadDishesForAdding('');
            });
        }
        
        // 3. Listener para el input de búsqueda (con debounce)
        let debounceTimeout;
        addDishSearchInput.addEventListener('input', () => {
            clearTimeout(debounceTimeout);
            debounceTimeout = setTimeout(() => {
                loadDishesForAdding(addDishSearchInput.value);
            }, 300); // Espera 300ms después de teclear
        });

        // 4. Listener para la lista de platos (delegación de eventos)
        addDishListContainer.addEventListener('click', handleAddDishClick);
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
        const historyStates = ['DELIVERY', 'CLOSED'];

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