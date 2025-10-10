// Te recomiendo poner esta función en un archivo separado, ej: Assets/JS/Render/render.js
// Y luego la importas donde la necesites.

export function renderCardPlato(plato) { 
    // 1. Creamos el DIV que servirá como columna en la grilla de Bootstrap
    const wrapper = document.createElement("div");
    wrapper.classList.add("col-lg-4", "col-md-6", "mb-4");
    
    // 2. Creamos el contenido de la card 
    wrapper.innerHTML = `
        <div class="card h-100">
            <img src="${plato.imageUrl}" class="card-img-top" alt="${plato.name}">
            <div class="card-body d-flex flex-column">
                <h5 class="card-title">${plato.name}</h5>
                <p class="card-text">${plato.description}</p>
                
                <div class="d-flex justify-content-between align-items-center mt-auto">
                    <p class="h4 m-0">$${plato.price}</p>
                    <button class="btn btn-primary">Agregar al pedido</button>
                </div>
            </div>
        </div>
    `;

    // 3. Devolvemos el elemento completo (columna + tarjeta)
    return wrapper;
}