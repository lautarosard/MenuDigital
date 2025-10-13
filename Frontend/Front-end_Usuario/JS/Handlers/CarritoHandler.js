// Este array ahora es "privado" de este módulo. Nadie más puede acceder a él directamente.
let carrito = [];

/**
 * Devuelve una copia del estado actual del carrito.
 * @returns {Array}
 */
export function getCarrito() {
    return [...carrito]; // Devuelve una copia para evitar mutaciones accidentales
}

/**
 * Añade un plato al carrito. Si ya existe, incrementa la cantidad.
 * @param {object} dish - El objeto completo del plato a añadir.
 */
export function agregarAlCarrito(dish) {
    const itemExistente = carrito.find(item => item.dish.id === dish.id);
    if (itemExistente) {
        itemExistente.quantity++;
    } else {
        carrito.push({ dish: dish, quantity: 1 });
    }
}

/**
 * Modifica la cantidad de un item en el carrito.
 * @param {string} dishId 
 * @param {number} change - Puede ser 1 para incrementar o -1 para decrementar.
 */
export function modificarCantidad(dishId, change) {
    const item = carrito.find(i => i.dish.id === dishId);
    if (!item) return;

    item.quantity += change;

    // Si la cantidad llega a 0 o menos, eliminamos el item
    if (item.quantity <= 0) {
        eliminarDelCarrito(dishId);
    }
}

/**
 * Elimina un item completo del carrito.
 * @param {string} dishId 
 */
export function eliminarDelCarrito(dishId) {
    carrito = carrito.filter(i => i.dish.id !== dishId);
}
export function limpiarCarrito() {
    carrito = [];
}