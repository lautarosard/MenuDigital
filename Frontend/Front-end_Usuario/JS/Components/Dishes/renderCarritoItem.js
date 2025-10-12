/**
 * Crea y devuelve el elemento HTML para un item en el carrito.
 * @param {object} item - El item del carrito (contiene dish y quantity).
 * @returns {HTMLElement} El elemento <div> de la fila del carrito.
 */
function renderCarritoItem(item) {
    const itemDiv = document.createElement('div');
    itemDiv.className = 'd-flex justify-content-between align-items-center mb-3 border-bottom pb-2';
    itemDiv.innerHTML = `
        <div class="d-flex align-items-center">
            <img src="${item.dish.imageUrl}" alt="${item.dish.name}" style="width: 60px; height: 60px; object-fit: cover;" class="rounded me-3">
            <div>
                <h6 class="mb-0">${item.dish.name}</h6>
                <small class="text-muted">$${item.dish.price}</small>
            </div>
        </div>
        <div class="d-flex align-items-center">
            <button class="btn btn-sm btn-outline-secondary btn-decrementar" data-dish-id="${item.dish.id}">-</button>
            <span class="mx-2">${item.quantity}</span>
            <button class="btn btn-sm btn-outline-secondary btn-incrementar" data-dish-id="${item.dish.id}">+</button>
            <button class="btn btn-sm btn-danger ms-3 btn-eliminar" data-dish-id="${item.dish.id}">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    `;
    return itemDiv;
}

/**
 * Actualiza toda la vista del carrito (badge y contenido del modal)
 * @param {Array} carrito - El estado actual del carrito.
 */
export function actualizarVistaCarrito(carrito) {
    const carritoBadge = document.getElementById('carrito-badge');
    const carritoItemsContainer = document.getElementById('carrito-items-container');
    
    carritoItemsContainer.innerHTML = '';

    if (carrito.length === 0) {
        carritoItemsContainer.innerHTML = '<p>Tu carrito está vacío.</p>';
        carritoBadge.style.display = 'none';
        return;
    }

    const totalItems = carrito.reduce((total, item) => total + item.quantity, 0);
    
    carritoBadge.textContent = totalItems;
    carritoBadge.style.display = 'inline';
    
    const fragment = document.createDocumentFragment();
    carrito.forEach(item => {
        const itemElement = renderCarritoItem(item);
        fragment.appendChild(itemElement);
    });
    
    carritoItemsContainer.appendChild(fragment);
}