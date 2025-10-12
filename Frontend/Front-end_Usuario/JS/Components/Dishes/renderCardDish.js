// Te recomiendo poner esta función en un archivo separado, ej: Assets/JS/Render/render.js
// Y luego la importas donde la necesites.

export function renderCardDish(dish) { 
    // 1. Creamos el DIV que servirá como columna en la grilla de Bootstrap
    const wrapper = document.createElement("div");
    wrapper.classList.add("col-lg-3", "col-md-4", "mb-4");
    
    // 2. Creamos el contenido de la card 
    // Dentro de tu función renderCarddish, reemplaza el HTML por este:
    wrapper.innerHTML = `
        <div class="card h-100 text-center">
            <img src="${dish.imageUrl}" class="card-img-top" alt="${dish.name}">
            <div class="card-body d-flex flex-column">
                <h5 class="card-title">${dish.name}</h5>
                <p class="card-text">${dish.description}</p>
            </div>
            <div class="card-footer">
                <p class="h4 m-0 mb-3">$${dish.price}</p>
                <button class="btn btn-primary w-100">
                    <i class="bi bi-cart-plus"></i> Agregar al pedido
                </button>
            </div>
        </div>
    `;

    // 3. Devolvemos el elemento completo (columna + tarjeta)
    return wrapper;
}