// En: JS/Pages/DishAdmin.js

import { getDishes } from '../APIs/DishApi.js'; 
import { renderCardDish } from '../Components/Dishes/renderCardDish.js'; 
import { getCategories } from '../APIs/CategoryApi.js';
//Referencias a los elementos del DOM que usaremos
const contenedorDishes = document.getElementById('dishes-container');
const inputBusqueda = document.getElementById('input-busqueda');
const categoryFiltersContainer = document.getElementById('category-filters-container');
const sortByPriceSelect = document.getElementById('sort-by-price');
const onlyActiveCheckbox = document.getElementById('only-active-checkbox');

//Creamos un objeto para guardar el estado de nuestros filtros
let currentFilters = {
    name: '',
    category: null,
    sortByPrice: 'ASC', // Valor por defecto
    onlyActive: null
};

let debounceTimeout;

/**
 * Función reutilizable que renderiza una lista de dishs en el contenedor.
 * @param {Array} DishList - La lista de dishs a mostrar.
 */
function DishRender(DishList) {
    // Limpiamos el contenedor
    contenedorDishes.innerHTML = '';

    // Si la lista está vacía, mostramos un mensaje
    if (DishList.length === 0) {
        contenedorDishes.innerHTML = '<p class="text-center">No se encontraron dishs.</p>';
        return;
    }

    const fragment = document.createDocumentFragment();
    DishList.forEach(dish => {
        const tarjetaElemento = renderCardDish(dish);
        fragment.appendChild(tarjetaElemento);
    });

    contenedorDishes.appendChild(fragment);
}

/**
 * Función que se encarga de llamar a la API con los filtros actuales y renderizar.
 */
async function applyFiltersAndRender() {
    contenedorDishes.innerHTML = '<p class="text-center">Buscando...</p>';
    //Pasamos el objeto de filtros completo a la API
    const dishes = await getDishes(currentFilters);
    DishRender(dishes);
}
function renderCategoryFilters(categories) {
    categoryFiltersContainer.innerHTML = ''; // Limpiamos por si acaso
    // Creamos el botón "Todos"
    const allButton = document.createElement('button');
    allButton.className = 'btn btn-secondary active'; // El primero está activo por defecto
    allButton.textContent = 'Todos';
    allButton.dataset.categoryId = ''; // Sin ID para mostrar todos
    categoryFiltersContainer.appendChild(allButton);

    // Creamos un botón por cada categoría de la API
    categories.forEach(category => {
        const button = document.createElement('button');
        button.className = 'btn btn-secondary';
        button.textContent = category.name;
        button.dataset.categoryId = category.id;
        categoryFiltersContainer.appendChild(button);
    });
}
/**
 * Función principal que se encarga de iniciar la lógica de esta página.
 */
async function inicializar() {
    console.log("Inicializando página de dishs...");
    try {
        // --- Carga de datos iniciales ---
        const categories = await getCategories();
        renderCategoryFilters(categories);
        await applyFiltersAndRender();

        // Carga inicial (podríamos pasar { onlyActive: true } por defecto, por ejemplo)
        await applyFiltersAndRender();

        // Configuramos el listener para la búsqueda por nombre
        inputBusqueda.addEventListener('input', () => {
            clearTimeout(debounceTimeout);
            debounceTimeout = setTimeout(() => {
                // ---> PASO 3: Actualizamos el estado de los filtros y volvemos a renderizar
                currentFilters.name = inputBusqueda.value.trim();
                applyFiltersAndRender();
            }, 300);
        });

        // Listener para los BOTONES DE CATEGORÍA (usando delegación de eventos)
        categoryFiltersContainer.addEventListener('click', (event) => {
            if (event.target.tagName === 'BUTTON') {
                // Actualizamos el filtro de categoría
                currentFilters.category = event.target.dataset.categoryId || null;
                applyFiltersAndRender();

                // Manejamos la clase 'active' para el feedback visual
                categoryFiltersContainer.querySelector('.active').classList.remove('active');
                event.target.classList.add('active');
            }
        });

        // Listener para el ORDEN POR PRECIO
        sortByPriceSelect.addEventListener('change', () => {
            currentFilters.sortByPrice = sortByPriceSelect.value;
            applyFiltersAndRender();
        });

        // Listener para el CHECKBOX DE SÓLO ACTIVOS
        onlyActiveCheckbox.addEventListener('change', () => {
            currentFilters.onlyActive = onlyActiveCheckbox.checked ? true : null;
            applyFiltersAndRender();
        });
    } catch (error) {
        console.error("Error al inicializar la página:", error);
        contenedorDishes.innerHTML = '<p class="text-center text-danger">No se pudo cargar el menú. Intente más tarde.</p>';
    }
}

// Exportamos la función para que main.js la llame
export default inicializar;