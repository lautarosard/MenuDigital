// En: JS/Components/Orders/renderOrderItemDetail.js

/**
 * Crea y devuelve el elemento HTML para un ítem DENTRO del modal de detalle.
 * @param {object} item - El objeto 'item' de la API (incluye 'dish', 'quantity', 'notes').
 * @returns {HTMLElement} - El elemento <div> del ítem.
 */
export function renderOrderItemDetail(item) {
    const itemCard = document.createElement('div');
    itemCard.className = 'p-3 border rounded shadow-sm bg-light item-card-container';
    
    
    itemCard.dataset.dishId = item.dish.id;

    
    // ==========================================================
    // === 2. ASEGÚRATE DE QUE EL INNERHTML ESTÉ ACTUALIZADO ===
    // (Sin 'readonly' y con las clases 'item-quantity-input' y 'item-notes-input')
    // ==========================================================
    itemCard.innerHTML = `
        <div class="d-flex justify-content-between align-items-center mb-2">
            <h6 class="mb-0 text-truncate">${item.dish.name || 'Nombre no disponible'}</h6>
            
            <div class="input-group" style="width: 130px;">
                <button class="btn btn-outline-secondary btn-item-decrease" type="button" data-item-id="${item.id}">-</button>
                
                <input 
                    type="number" 
                    class="form-control text-center item-quantity-input" 
                    value="${item.quantity}" 
                    min="0">
                
                <button class="btn btn-outline-secondary btn-item-increase" type="button" data-item-id="${item.id}">+</button>
            </div>
        </div>

        <textarea 
            class="form-control form-control-sm item-notes-input" 
            rows="1" 
            placeholder="Notas para este plato..." 
        >${item.notes || ''}</textarea>
    `;

    return itemCard;
}