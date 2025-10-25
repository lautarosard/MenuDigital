// En: JS/Components/Admin/renderOrderPanelCard.js

import { renderOrderItemPanel } from './renderOrderItemPanel.js';

/**
 * Mapeo de tipos de entrega para mostrar.
 * Adapta los IDs y nombres a los de tu API.
 */
const deliveryTypeMap = {
    1: { text: 'Delivery', color: 'primary' },
    2: { text: 'Take away', color: 'info' },
    3: { text: 'Dine in', color: 'secondary' },
    default: { text: 'Desconocido', color: 'dark' }
};

/**
 * Renderiza la tarjeta completa de una orden para una columna del panel.
 * @param {object} order - El objeto completo de la orden de la API.
 * @returns {HTMLElement} - El elemento <div> de la tarjeta.
 */
export function renderOrderPanelCard(order) {
    const card = document.createElement('div');
    card.className = 'card mb-3 shadow-sm order-card';
    card.dataset.orderId = order.orderNumber; // Guardamos el ID de la orden

    // Obtenemos info del tipo de entrega
    const deliveryInfo = deliveryTypeMap[order.deliveryType.id] || deliveryTypeMap.default;

    // Generamos los botones de ACCIÓN GENERAL según el estado
    let orderActionButtonsHtml = '';
    if (order.status.id === 1) { // Pendiente
        orderActionButtonsHtml = `
            <button class="btn btn-sm btn-outline-danger btn-cancel-order" title="Cancelar Orden Completa">Cancelar Orden</button>
            <button class="btn btn-sm btn-primary btn-view-details" title="Ver Detalles">Ver Detalle</button>
        `;
    } else if (order.status.id === 2) { // En Preparación
         orderActionButtonsHtml = `<button class="btn btn-sm btn-primary btn-view-details" title="Ver Detalles">Ver Detalle</button>`;
    } else if (order.status.id === 3) { // Listo para Entregar
        orderActionButtonsHtml = `
            <button class="btn btn-sm btn-dark btn-deliver-order" title="Marcar Orden como Entregada">Entregar</button>
            <button class="btn btn-sm btn-primary btn-view-details" title="Ver Detalles">Ver Detalle</button>
        `;
    }

    // Renderizamos los ítems usando el otro componente
    const itemsHtml = order.items.map(item => 
        renderOrderItemPanel(item, order.status.id).outerHTML
    ).join('');

    // Construimos el HTML completo de la tarjeta
    card.innerHTML = `
        <div class="card-header d-flex justify-content-between align-items-center">
            <strong class="me-2">Orden #${order.orderNumber}</strong>
            <span class="badge bg-${deliveryInfo.color}">${deliveryInfo.text}</span>
        </div>
        <ul class="list-group list-group-flush">
            ${itemsHtml || '<li class="list-group-item text-muted text-center">Sin ítems</li>'}
        </ul>
        <div class="card-footer text-end">
            <div class="btn-group btn-group-sm">
                ${orderActionButtonsHtml}
            </div>
        </div>
    `;

    return card;
}