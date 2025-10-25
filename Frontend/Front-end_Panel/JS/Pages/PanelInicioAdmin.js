// En: JS/Pages/PanelInicio.js

// 1. Importaciones
import { getOrders, updateOrderItems, updateOrderItemStatus, cancelOrder, deliverOrder } from "./../APIs/OrderApi.js";
import { renderOrderPanelCard } from "./../Components/renderOrderPanelCard.js";

// 2. Referencias a los contenedores de columnas
const pendingContainer = document.getElementById('pending-orders-container');
const inProgressContainer = document.getElementById('inprogress-orders-container');
const readyContainer = document.getElementById('ready-orders-container');

// IDs de estado (¡AJUSTA ESTOS A TU API!)
const STATUS_PENDING = 1;
const STATUS_IN_PROGRESS = 2;
const STATUS_READY = 3;
const STATUS_DELIVERED = 4; // Asumiendo 4=Delivered/Completed
const STATUS_CANCELLED = 5; // Asumiendo 5=Cancelled

/**
 * Carga las órdenes desde la API y las renderiza en las columnas.
 */
async function loadAndRenderOrders() {
    // Limpiamos y ponemos 'Cargando...'
    [pendingContainer, inProgressContainer, readyContainer].forEach(container => {
        if(container) container.innerHTML = '<p class="text-center text-muted">Cargando...</p>';
    });

    try {
        // Pedimos TODAS las órdenes activas (Pending, InProgress, Ready)
        const allRelevantOrders = await getOrders(); 
        
        // Filtramos en el front-end
        const pendingOrders = allRelevantOrders.filter(o => o.status.id === STATUS_PENDING);
        const inProgressOrders = allRelevantOrders.filter(o => o.status.id === STATUS_IN_PROGRESS);
        const readyOrders = allRelevantOrders.filter(o => o.status.id === STATUS_READY);

        // Renderizamos en cada columna
        renderColumn(pendingContainer, pendingOrders);
        renderColumn(inProgressContainer, inProgressOrders);
        renderColumn(readyContainer, readyOrders);

    } catch (error) {
        console.error("Error al cargar órdenes para el panel:", error);
        [pendingContainer, inProgressContainer, readyContainer].forEach(container => {
            if(container) container.innerHTML = '<p class="text-center text-danger">Error al cargar órdenes.</p>';
        });
    }
}

//REVISAR
async function showOrderDetails(orderId) {
    try {
        // 1️⃣ Obtener la orden por ID
        const response = await axios.get(`https://localhost:7280/api/v1/Order/${orderId}`);
        const order = response.data;

        // 2️⃣ Armar el HTML de detalles
        const html = `
            <p><strong>Número de Orden:</strong> ${order.orderNumber}</p>
            <p><strong>Estado:</strong> ${order.status.name}</p>
            <p><strong>Tipo de Entrega:</strong> ${order.deliveryType.name}</p>
            <p><strong>Total:</strong> $${order.totalAmount.toFixed(2)}</p>
            <hr>
            <h6>Ítems:</h6>
            <ul class="list-group">
                ${order.items.map(item => `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        <div>
                            <strong>${item.dish.name}</strong><br>
                            <small>Cantidad: ${item.quantity}</small><br>
                            <small>Estado: ${item.status.name}</small>
                        </div>
                        <img src="${item.dish.image}" alt="${item.dish.name}" style="width:60px;height:60px;border-radius:8px;">
                    </li>
                `).join('')}
            </ul>
        `;

        // 3️⃣ Inyectarlo en el modal
        document.getElementById("orderDetailsContent").innerHTML = html;

        // 4️⃣ Mostrar modal
        const modal = new bootstrap.Modal(document.getElementById('orderDetailsModal'));
        modal.show();
    } catch (error) {
        console.error("Error al obtener detalles de la orden:", error.response?.data || error.message);
        alert("No se pudieron cargar los detalles de la orden.");
    }
}
/**
 * Ayudante para renderizar órdenes en una columna específica.
 */
function renderColumn(container, orders) {
    if (!container) return;
    container.innerHTML = ''; // Limpiar
    if (orders.length === 0) {
        container.innerHTML = '<p class="text-center text-muted">No hay órdenes en este estado.</p>';
        return;
    }
    orders.forEach(order => {
        const card = renderOrderPanelCard(order);
        container.appendChild(card);
    });
}

/**
 * Maneja los clics en TODOS los botones de acción del panel.
 */
async function handlePanelActions(event) {
    const target = event.target;
    const orderCard = target.closest('.order-card');
    const orderId = orderCard?.dataset.orderId;
    const itemId = target.dataset.itemId;

    if (!orderId && !itemId) return; // No se hizo clic en algo accionable

    try {
        // Acciones a nivel de ORDEN
        if (target.classList.contains('btn-cancel-order')) {
            if (confirm(`¿Seguro que quieres CANCELAR la orden #${orderId}?`)) {
                await cancelOrder(orderId); // Llama a la API (cambia estado a Cancelled)
            }
        } else if (target.classList.contains('btn-deliver-order')) {
            if (confirm(`¿Marcar la orden #${orderId} como ENTREGADA?`)) {
                await deliverOrder(orderId);
            }
        } // <- Se corrigió el 'else if' que estaba mal anidado
        else if (target.classList.contains('btn-view-details')) {
                await showOrderDetails(orderId);
        }

        // Acciones a nivel de ÍTEM
        else if (target.classList.contains('btn-prepare-item')) {
            // CORREGIDO: Usamos 'status'
            await updateOrderItemStatus(orderId, itemId, { status: STATUS_IN_PROGRESS });
        } else if (target.classList.contains('btn-finish-item')) {
            // Cambia estado del ÍTEM a "Ready" (ID 3)
            // CORREGIDO: Usamos 'status'
            await updateOrderItemStatus(orderId, itemId, { status: STATUS_READY });
        }
        
        // Si alguna acción tuvo éxito (no lanzó error), refrescamos TODO el panel
        await loadAndRenderOrders(); 
        
    } catch (error) {
        alert("Error al procesar la acción. Revisa la consola.");
        console.error("Error capturado en handlePanelActions:", error);
    }
} // <- Esta es la llave que cierra handlePanelActions


/**
 * Función de arranque para la página del Panel de Órdenes.
 */
async function inicializarPanelInicio() {
    console.log("Inicializando Panel de Órdenes...");
    
    // Carga inicial
    await loadAndRenderOrders();

    // Listener general para todas las acciones (delegación)
    // Escuchamos en el 'main' para capturar clics en CUALQUIER columna
    document.querySelector('main').addEventListener('click', handlePanelActions);

    // (Opcional) Refresco automático cada X segundos
    // setInterval(loadAndRenderOrders, 30000); // Refresca cada 30 segundos
}

export default inicializarPanelInicio;
