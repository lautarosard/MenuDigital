// En: JS/Handlers/OrderHandler.js

import * as CartHandler from './CarritoHandler.js';

/**
 * Recolecta los datos del carrito y del formulario del DOM para construir
 * el objeto de solicitud de orden que espera la API.
 * @returns {object | null} El objeto orderRequest o null si hay un error.
 */
export function buildOrderRequest() {
    const carritoActual = CartHandler.getCarrito();
    if (carritoActual.length === 0) {
        alert("Tu carrito está vacío.");
        return null;
    }

    const items = carritoActual.map(item => ({
        id: item.dish.id,
        quantity: item.quantity,
        notes: "" // Notas por item, si las tuvieras
    }));
    
    const selectedDeliveryType = document.querySelector('input[name="deliveryType"]:checked');
    if (!selectedDeliveryType) {
        alert("Por favor, selecciona un tipo de entrega.");
        return null;
    }
    
    // El ID que enviaremos a la API
    const deliveryId = parseInt(selectedDeliveryType.value);
    let deliveryToValue = "";

    // === LÓGICA CORREGIDA PARA COINCIDIR CON EL HTML Y LA API ===
    if (deliveryId === 1) { // 1 = Delivery
        deliveryToValue = document.getElementById('delivery-address').value;
    } else if (deliveryId === 2) { // 2 = Retiro en local
        deliveryToValue = document.getElementById('diner-name').value;
    } else if (deliveryId === 3) { // 3 = Comida en el local
        deliveryToValue = document.getElementById('table-number').value;
    }
    // ==========================================================

    const delivery = {
        id: deliveryId,
        to: deliveryToValue
    };
    
    const orderRequest = {
        items: items,
        delivery: delivery,
        notes: document.getElementById('order-notes').value || ""
    };
    
    return orderRequest;
}