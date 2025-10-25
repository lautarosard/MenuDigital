// En: JS/Components/Orders/renderAddDishItem.js

export function renderAddDishItem(dish) {
    const dishItem = document.createElement('a');
    dishItem.className = 'list-group-item list-group-item-action d-flex justify-content-between align-items-center btn-add-dish-to-order';
    dishItem.href = '#'; // Para que parezca clickeable
    
    // Guardamos TODOS los datos del plato usando data-attributes
    dishItem.dataset.dishId = dish.id;
    dishItem.dataset.dishName = dish.name;
    // (Añadiremos más si los necesitamos)

    // Formateamos el precio
    const priceFormatted = (dish.price || 0).toLocaleString('es-AR', {
        style: 'currency',
        currency: 'ARS'
    });

    dishItem.innerHTML = `
        <span>
            <h6 class="mb-0">${dish.name}</h6>
            <small class="text-muted">${dish.categoryName || 'Sin categoría'}</small>
        </span>
        <strong class="text-success">${priceFormatted}</strong>
    `;
    
    return dishItem;
}