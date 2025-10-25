// En: JS/Components/Admin/renderOrderItemPanel.js

/**
 * Mapeo simple de estados de ÍTEM para mostrar.
 * Adapta los IDs y nombres a los de tu API.
 */
const itemStatusMap = {
    1: { text: 'Pending', color: 'secondary' },
    2: { text: 'In progress', color: 'primary' },
    3: { text: 'Ready', color: 'success' },
    // Añade más si los necesitas (ej: Cancelled)
    default: { text: 'Desconocido', color: 'dark' }
};

/**
 * Renderiza un solo ítem dentro de la tarjeta de orden del panel.
 * @param {object} item - El objeto item de la orden (incluye dish, quantity, status).
 * @param {number} orderStatusId - El ID del estado GENERAL de la orden (para saber qué botón mostrar).
 * @returns {HTMLElement} - El elemento <li> del ítem.
 */
export function renderOrderItemPanel(item, orderStatusId) {
    const li = document.createElement('li');
    li.className = 'list-group-item d-flex justify-content-between align-items-center';
    
    // Obtenemos el texto y color del estado del ÍTEM
    const itemStatusInfo = itemStatusMap[item.status.id] || itemStatusMap.default;
    
    // Texto principal del ítem
    let itemText = `${item.quantity}x ${item.dish.name}`;
    
    // Añadimos el estado del ítem si es relevante (ej: para "En Preparación")
    if (orderStatusId === 2) { // Asumiendo que 2 = En Preparación
        itemText += ` <small class="text-${itemStatusInfo.color}">(${itemStatusInfo.text})</small>`;
    }

    // Definimos el botón de acción según el estado GENERAL de la orden
    let actionButtonHtml = '';
    if (orderStatusId === 1) { // Pendiente
        actionButtonHtml = `
            <button class="btn btn-sm btn-warning btn-prepare-item" data-item-id="${item.id}" title="Marcar como 'En Preparación'">
                Preparar
            </button>
        `;
    } else if (orderStatusId === 2 && item.status.id !== 3) { // En Preparación y el ítem NO está listo
        actionButtonHtml = `
            <button class="btn btn-sm btn-success btn-finish-item" data-item-id="${item.id}" title="Marcar como 'Listo'">
                Terminar
            </button>
        `;
    }
    // No ponemos botón si la orden está Lista o si el ítem ya está Listo

    li.innerHTML = `
        <span>${itemText}</span>
        <div class="btn-group btn-group-sm">
            ${actionButtonHtml}
        </div>
    `;
    
    return li;
}