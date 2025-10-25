// En: JS/Components/Admin/renderDishCardAdmin.js
function formatPrice(price) {
    return (price || 0).toLocaleString('es-AR', {
        style: 'currency',
        currency: 'ARS'
    });
}

export function renderDishCardAdmin(dish) {
    
    // 1. Crear el contenedor (la columna)
    const col = document.createElement('div');
    col.className = 'col-md-6 col-lg-4 mb-4';

    // 2. Definir el HTML interno de la tarjeta
    // Usamos clases de Bootstrap 'shadow-sm', 'h-100' (para altura completa)
    
    // 3. Lógica para el estado (Activo/Inactivo)
    const statusBadge = dish.isActive
        ? `<span class="badge bg-success">Activo</span>`
        : `<span class="badge bg-danger">Inactivo</span>`;

    // 4. Lógica para los botones (Activar/Desactivar)
    const actionButton = dish.isActive
        ? `<button class="btn btn-sm btn-danger btn-desactivar" data-dish-id="${dish.id}">Desactivar</button>`
        : `<button class="btn btn-sm btn-success btn-activar" data-dish-id="${dish.id}">Activar</button>`;

    col.innerHTML = `
        <div class="card h-100 shadow-sm">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <h5 class="card-title">${dish.name}</h5>
                        <p class="card-text text-muted">${formatPrice(dish.price)}</p>
                    </div>
                    ${statusBadge}
                </div>
            </div>
            <div class="card-footer bg-light border-0 text-end">
                <button class="btn btn-sm btn-secondary btn-editar" data-dish-id="${dish.id}">Editar</button>
                ${actionButton}
            </div>
        </div>
    `;

    return col;
}