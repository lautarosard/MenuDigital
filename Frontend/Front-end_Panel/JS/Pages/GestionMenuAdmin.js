// En: JS/Pages/GestionMenu.js

import { getDishes, UpdateDish, createDish } from './../APIs/DishApi.js';
import { getCategories } from './../APIs/CategoryApi.js';
import { renderDishCardAdmin } from './../Components/renderDishCardAdmin.js';
// 2. Referencias del DOM
const container = document.getElementById('admin-dishes-container');

let allDishes = [];

const modalElement = document.getElementById('dish-modal');
const modalTitle = document.getElementById('dish-modal-title');
const dishForm = document.getElementById('dish-form');
const categorySelect = document.getElementById('dish-category');
let dishModalInstance = null;

let editingDishId = null;

function populateDishModalForEdit(dish) {
    document.getElementById('dish-name').value = dish.name;
    document.getElementById('dish-price').value = dish.price;
    document.getElementById('dish-category').value = dish.category.id; // Asigna el ID de la categoría
    document.getElementById('dish-description').value = dish.description || '';
    document.getElementById('dish-image').value = dish.imageUrl || ''; // Usa imageUrl
    document.getElementById('dish-isActive').checked = dish.isActive;
}
async function handleDishFormSubmit(event) {
    event.preventDefault();
    event.stopPropagation();

    if (!dishForm.checkValidity()) {
        dishForm.classList.add('was-validated');
        return;
    }

    const btnGuardar = document.getElementById('btn-guardar-plato');
    btnGuardar.disabled = true;
    btnGuardar.textContent = 'Guardando...';

    // Construimos el objeto DishRequest / DishUpdateRequest
    const dishData = {
        name: document.getElementById('dish-name').value,
        description: document.getElementById('dish-description').value || null,
        price: parseFloat(document.getElementById('dish-price').value),
        category: parseInt(categorySelect.value),
        image: document.getElementById('dish-image').value || null,
        isActive: document.getElementById('dish-isActive').checked
    };

    try {
        let successMessage = ''; // Para mostrar el mensaje correcto

        if (editingDishId) {
            // === ESTAMOS EDITANDO ===
            await UpdateDish(editingDishId, dishData);
            successMessage = '¡Plato actualizado con éxito!';
        } else {
            // === ESTAMOS CREANDO ===
            // (Tu lógica opcional para quitar 'isActive' aquí)
            // delete dishData.isActive; 
            
            await createDish(dishData);
            successMessage = '¡Plato creado con éxito!';
        }

        editingDishId = null; 

        alert(successMessage); // Mostramos el mensaje correcto
        dishModalInstance.hide();
        loadAllDishes(); // Recargar la lista

    } catch (error) {
        alert(`Error al guardar el plato. Revisa la consola.`);
        console.error("Error capturado al guardar plato:", error);
    } finally {
        btnGuardar.disabled = false;
        btnGuardar.textContent = 'Guardar Plato';
        dishForm.classList.remove('was-validated');
    }
}
/**
 * Función principal para cargar y renderizar todos los platos.
 */
async function loadAllDishes() {
    container.innerHTML = '<p class="text-center">Cargando platos...</p>';
    
    try {
        allDishes = await getDishes({ onlyActive: false });

        if (allDishes.length === 0) {
            container.innerHTML = '<p class="text-center">No se encontraron platos.</p>';
            return;
        }

        // 3. Renderizar
        container.innerHTML = ''; // Limpiamos el "Cargando..."
        const fragment = document.createDocumentFragment();
        allDishes.forEach(dish => {
            const card = renderDishCardAdmin(dish);
            fragment.appendChild(card);
        });
        container.appendChild(fragment);

    } catch (error) {
        console.error("Error al cargar los platos:", error);
        container.innerHTML = '<p class="text-center text-danger">No se pudo cargar la gestión de menú.</p>';
    }
}

async function handleToggleActive(dishId, newStatus) {
    // 1. Encontrar el plato completo en nuestra lista
    const dishToUpdate = allDishes.find(dish => dish.id === dishId);
    
    // 2. Verificación de seguridad (usando la ruta correcta del objeto)
    // Comprueba si 'category' o 'category.id' no existen
    if (!dishToUpdate.category || dishToUpdate.category.id === undefined || dishToUpdate.category.id === null) {
        
        // (Cambié el mensaje de alerta para que sea más claro)
        alert('Error: El objeto del plato no tiene un "category.id" válido. No se puede actualizar.');
        console.error("Objeto 'dishToUpdate' inválido (falta category.id):", dishToUpdate);
        return;
    }

    // 3. Construir manualmente el objeto 'DishUpdateRequest'
    // Mapeamos los nombres del objeto de respuesta (ej: imageUrl)
    // a los nombres del objeto de solicitud (ej: category, isActive)
    const dishRequestObject = {
        name: dishToUpdate.name,
        description: dishToUpdate.description,
        price: dishToUpdate.price,
        image: dishToUpdate.imageUrl,         // Mapea 'imageUrl' a 'image'
        category: dishToUpdate.category.id, // Mapea 'category.id' a 'category'
        isActive: newStatus                  // Usa 'isActive'
    };

    try {
        // 4. Llamamos a la API con el objeto 'DishUpdateRequest' construido
        // (Asegurándonos de que 'UpdateDish' (minúscula) esté importado)
        await UpdateDish(dishId, dishRequestObject);
        
        alert(`Plato ${newStatus ? 'activado' : 'desactivado'} con éxito.`);
        loadAllDishes(); // Recargamos

    } catch (error) {
        alert('Error al actualizar el estado del plato. Revisa la consola.');
    }
}

async function loadCategoriesIntoModal() {
    categorySelect.innerHTML = '<option value="" disabled>Cargando...</option>';
    
    const categories = await getCategories();
    
    categorySelect.innerHTML = '<option value="" selected disabled>Selecciona una categoría</option>';
    if (categories.length === 0) {
        categorySelect.innerHTML = '<option value="" disabled>No se encontraron categorías</option>';
        return;
    }

    categories.forEach(category => {
        const option = document.createElement('option');
        option.value = category.id;
        option.textContent = category.name;
        categorySelect.appendChild(option);
    });
}

/**
 * Maneja el envío del formulario para crear un nuevo plato.
 */
async function handleCreateDishSubmit(event) {
    event.preventDefault(); // Evita que la página se recargue
    event.stopPropagation(); // Evita validaciones HTML5 fantasmas

    // 1. Verificar validez (Bootstrap)
    if (!dishForm.checkValidity()) {
        dishForm.classList.add('was-validated');
        return;
    }

    const btnGuardar = document.getElementById('btn-guardar-plato');
    btnGuardar.disabled = true;
    btnGuardar.textContent = 'Guardando...';

    try {
        // 2. Construir el objeto 'DishRequest'
        const dishRequest = {
            name: document.getElementById('dish-name').value,
            description: document.getElementById('dish-description').value || null,
            price: parseFloat(document.getElementById('dish-price').value),
            category: parseInt(categorySelect.value), // El C# espera 'Category' (int)
            image: document.getElementById('dish-image').value || null,
            // 'isActive' no está en DishRequest, lo omitimos.
        };

        // 3. Llamar a la API
        await createDish(dishRequest);

        alert('¡Plato creado con éxito!');
        dishModalInstance.hide(); // Esconder el modal
        loadAllDishes(); // Recargar la lista de platos

    } catch (error) {
        alert('Error al crear el plato. Revisa la consola.');
    } finally {
        btnGuardar.disabled = false;
        btnGuardar.textContent = 'Guardar Plato';
        dishForm.classList.remove('was-validated'); // Limpiar validación
    }
}

/**
 * Función de arranque para la página de Gestión de Menú.
 */
async function inicializarGestionMenu() {
    console.log("Inicializando página de Gestión de Menú...");
    dishModalInstance = new bootstrap.Modal(modalElement);
    // 6. Carga inicial
    loadAllDishes();

    // 7. Listeners (Delegación de eventos)
    container.addEventListener('click', (event) => {
        const target = event.target;
        const dishId = target.dataset.dishId;

        if (!dishId) return; // No se hizo clic en un botón con ID

        // Clic en "Desactivar"
        if (target.classList.contains('btn-desactivar')) {
            if (confirm('¿Estás seguro de que quieres DESACTIVAR este plato?')) {
                handleToggleActive(dishId, false);
            }
        }

        // Clic en "Activar"
        if (target.classList.contains('btn-activar')) {
            handleToggleActive(dishId, true);
        }

        if (target.classList.contains('btn-editar')) {
            // 1. Buscar el plato completo
            const dishToEdit = allDishes.find(dish => dish.id === dishId);
            if (!dishToEdit) {
                alert('Error: No se encontró el plato para editar.');
                return;
            }
            
            // 2. Preparar el modal para "Editar"
            dishForm.reset();
            dishForm.classList.remove('was-validated');
            modalTitle.textContent = `Editar Plato: ${dishToEdit.name}`;
            editingDishId = dishId; // Guardamos el ID que estamos editando
            
            // 3. Cargar categorías y rellenar el formulario
            loadCategoriesIntoModal().then(() => {
                populateDishModalForEdit(dishToEdit); // Rellenamos DESPUÉS de cargar categorías
            });
            
            // 4. Mostrar el modal
            dishModalInstance.show();
        }
    });

    document.getElementById('btn-anadir-plato').addEventListener('click', () => {
        // Preparamos el modal para "Crear"
        dishForm.reset(); // Limpiamos el formulario
        dishForm.classList.remove('was-validated');
        modalTitle.textContent = 'Añadir Nuevo Plato';
        editingDishId = null;
        // Cargamos las categorías CADA VEZ que se abre
        loadCategoriesIntoModal();
        
        // Mostramos el modal
        dishModalInstance.show();
    });

    // Listener para el envío (submit) del formulario
    dishForm.addEventListener('submit', handleDishFormSubmit);
}

export default inicializarGestionMenu;