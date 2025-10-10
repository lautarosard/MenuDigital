const API_BASE = "https://localhost:7280/api/v1/Order";

export async function getOrders() {
    try {
        const response = await axios.get(API_BASE);
        if (response.data && Array.isArray(response.data)) {
            return response.data;
        } else {
            console.warn("No se encontraron platos en la respuesta de la API.");
            return [];
        }
    } catch (error) {
        console.error("Error al obtener los platos:", error);
        return [];
    }
}

export async function getOrderById(id) {
    const response = await axios.get(`${API_BASE}/${id}`);
}
export async function createOrder(order) {
    try {
        const response = await axios.post(API_BASE, order);
        return response.data;
    } catch (error) {
        console.error("Error al crear el plato:", error);
        return null;
    }
}

export async function UpdateOrderItems(id, orderUpdated) {
    try {
        const response = await axios.patch(`${API_BASE}/${id}`, orderUpdated, {
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return response.data;
    } catch (error) {
        console.error("Error al actualizar el plato:", error);
        return null;
    }
}

export async function UpdateOrderItemStatus(orderId, itemId, OrderItemUpdateRequest) {
    try {
        const response = await axios.patch(`${API_BASE}/${orderId}/items/${itemId}`, OrderItemUpdateRequest, {
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return response.data;
    } catch (error) {
        console.error("Error al actualizar el plato:", error);
        return null;
    }
}
export async function deleteOrder(id) {
    try {
        const response = await axios.delete(`${API_BASE}/${id}`);
        return response.data;
    } catch (error) {
        console.error("Error al eliminar el plato:", error);
        return null;
    }
}
