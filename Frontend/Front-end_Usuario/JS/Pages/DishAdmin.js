// En: JS/Pages/DishAdmin.js

import { getDishes } from '../APIs/DishApi.js'; 
import { renderCardDish } from '../Components/Dishes/renderCardDish.js'; 
import { getCategories } from '../APIs/CategoryApi.js';
import * as CartHandler from '../Handlers/CarritoHandler.js';
import { actualizarVistaCarrito } from '../Components/Dishes/renderCarritoItem.js';
import { buildOrderRequest } from '../Handlers/OrderHandler.js';
import * as OrderApi from './../APIs/OrderApi.js';

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
    const orders = await OrderApi.getOrders();
    console.log(orders);
    try {
        // --- Carga de datos iniciales ---
        const categories = await getCategories();
        renderCategoryFilters(categories);

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
        // === INICIO: CÓDIGO NUEVO PARA INPUTS DE TIPO DE ENTREGA ===
        // ===================================================================

        // 1. Obtenemos referencias a los contenedores de los inputs
        const tableInputContainer = document.getElementById('input-container-table');
        const nameInputContainer = document.getElementById('input-container-name');
        const addressInputContainer = document.getElementById('input-container-address');

        // 2. Obtenemos referencias a TODOS los radio buttons de tipo de entrega
        const deliveryTypeRadios = document.querySelectorAll('input[name="deliveryType"]');

        // 3. Creamos una función para actualizar la visibilidad
        function actualizarVisibilidadInputs() {
            // Obtenemos el valor (1, 2 o 3) del radio button que está SELECCIONADO
            const selectedValue = document.querySelector('input[name="deliveryType"]:checked').value;

            // Ocultamos todos los contenedores primero (para "resetear")
            tableInputContainer.style.display = 'none';
            nameInputContainer.style.display = 'none';
            addressInputContainer.style.display = 'none';

            // 4. Mostramos SOLO el contenedor correspondiente al valor
            if (selectedValue === '3') { // 3 = Consumir en el Salón
                tableInputContainer.style.display = 'block';
            } else if (selectedValue === '2') { // 2 = Para Llevar
                nameInputContainer.style.display = 'block';
            } else if (selectedValue === '1') { // 1 = Delivery
                addressInputContainer.style.display = 'block';
            }
        }

        // 5. Asignamos el listener a CADA radio button
        // Usamos 'change' para que se active cuando cambie la selección
        deliveryTypeRadios.forEach(radio => {
            radio.addEventListener('change', actualizarVisibilidadInputs);
        });
        
        // NOTA: No es necesario llamar a la función al inicio porque
        // tu HTML ya tiene 'checked' el radio 'Consumir en el Salón' (value="3")
        // y el 'input-container-table' es visible por defecto, así que ya coinciden.

        // ===================================================================
        // === FIN: CÓDIGO NUEVO PARA INPUTS DE TIPO DE ENTREGA ===
        // Listener para los botones "Agregar al pedido"
        contenedorDishes.addEventListener('click', (event) => {
            const botonAgregar = event.target.closest('.btn-agregar-pedido');
            if (botonAgregar) {
                const dishData = { 
                    id: botonAgregar.dataset.dishId,
                    name: botonAgregar.dataset.dishName,
                    price: parseFloat(botonAgregar.dataset.dishPrice),
                    imageUrl: botonAgregar.dataset.dishImageUrl
                };
                
                // --- Llama a la lógica del Handler ---
                CartHandler.agregarAlCarrito(dishData);
                // --- Llama a la vista del Render ---
                actualizarVistaCarrito(CartHandler.getCarrito());
            }
        });
            
        // Listener para los botones DENTRO del modal del Order
        const carritoModalBody = document.getElementById('carrito-items-container');
        carritoModalBody.addEventListener('click', (event) => {
            const target = event.target;
            const dishId = target.dataset.dishId;
            if (!dishId) return;

            if (target.classList.contains('btn-incrementar')) {
                CartHandler.modificarCantidad(dishId, 1);
            }
            if (target.classList.contains('btn-decrementar')) {
                CartHandler.modificarCantidad(dishId, -1);
            }
            if (target.closest('.btn-eliminar')) {
                CartHandler.eliminarDelCarrito(dishId);
            }
            
            // Después de cualquier cambio, actualizamos la vista
            actualizarVistaCarrito(CartHandler.getCarrito());
        });
        // Listener para el botón de CONFIRMAR PEDIDO (Versión Modular)
        const confirmarPedidoBtn = document.getElementById('confirmar-pedido-btn');
        confirmarPedidoBtn.addEventListener('click', async () => {
            
            // 1. Le pedimos al Handler que construya el objeto del pedido
            const orderRequest = buildOrderRequest();

            // Si buildOrderRequest devuelve null (carrito vacío o falta tipo de entrega), no hacemos nada
            if (!orderRequest) {
                return;
            }

            console.log("Enviando orden a la API:", orderRequest);

            // 2. Intentamos enviar el pedido y manejamos la respuesta
            try {
                // <-- CORRECCIÓN: Ahora `OrderApi` está definido y se puede llamar
                const respuesta = await OrderApi.createOrder(orderRequest);
                alert(`¡Pedido creado con éxito! Número de orden: ${respuesta.orderNumber}`);
                
                // 3. Acciones de éxito
                CartHandler.limpiarCarrito();
                actualizarVistaCarrito(CartHandler.getCarrito());

                const modal = bootstrap.Modal.getInstance(document.getElementById('carritoModal'));
                modal.hide();

            } catch (apiError) {
                // Mejoramos el mensaje de error para que sea más claro
                const errorMessage = apiError.response?.data?.message || "Ocurrió un error al procesar el pedido.";
                alert(`Error al crear el pedido:\n${errorMessage}`);
            }
        });

    } catch (error) {
        console.error("Error al inicializar la página:", error);
        contenedorDishes.innerHTML = '<p class="text-center text-danger">No se pudo cargar el menú. Intente más tarde.</p>';
    }
}

// Exportamos la función para que main.js la llame
export default inicializar;