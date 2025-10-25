const API_BASE = "https://localhost:7280/api/v1/Order";

export async function createOrder(orderRequest) {
    try {
        const response = await axios.post(API_BASE, orderRequest, {
            headers: { "Content-Type": "application/json" },
        });
        return response.data;
    } catch (error) {
        console.error("Error al crear la orden:", error.response?.data || error.message);
        throw error; // Re-lanzamos el error para que el componente que llama lo maneje.
    }
}

/**
 * Obtiene una lista de órdenes con filtros opcionales.
 * @param {object} filters - Un objeto con los filtros.
 * @param {number} [filters.statusId] - Filtrar por el ID del estado de la orden[cite: 85].
 * @param {string} [filters.from] - Fecha de inicio para el filtro (formato ISO 8601)[cite: 83].
 * @param {string} [filters.to] - Fecha de fin para el filtro (formato ISO 8601)[cite: 84].
 * @returns {Promise<Array>} Una lista de órdenes.
 */
export async function getOrders(filters = {}) {
    try {
        // Construimos los parámetros de la URL dinámicamente
        const params = new URLSearchParams();
        if (filters.statusId) params.append('statusId', filters.statusId);
        if (filters.from) params.append('from', filters.from);
        if (filters.to) params.append('to', filters.to);

        const response = await axios.get(API_BASE, { params });
        
        return response.data || []; // Devuelve los datos o un array vacío si no hay nada.
    } catch (error) {
        console.error("Error al obtener las órdenes:", error.response?.data || error.message);
        return [];
    }
}

/**
 * Obtiene una orden por su ID.
 * @param {number} id - El número de la orden.
 * @returns {Promise<object|null>} Los detalles de la orden o null si no se encuentra.
 */
export async function getOrderById(id) {
    try {
        const response = await axios.get(`${API_BASE}/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Error al obtener la orden ${id}:`, error.response?.data || error.message);
        return null;
    }
}

export async function updateOrderItems(orderId, orderUpdated) {
    try {
        const response = await axios.patch(`${API_BASE}/${orderId}`, orderUpdated, {
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return response.data;
    } catch (error) {
        console.error(`Error al actualizar la orden ${orderId}:`, error.response?.data || error.message);
        throw error;
    }
}

export async function updateOrderItemStatus(orderId, itemId, itemUpdate) {
    try {
        // Aceptamos ambas formas: { status: 5 } o { statusId: 5 }
        const statusValue = (itemUpdate && (itemUpdate.status ?? itemUpdate.statusId));
        if (typeof statusValue === 'undefined') {
            throw new Error('updateOrderItemStatus: no se proporcionó "status" ni "statusId" en itemUpdate');
        }

        // El backend espera un objeto con { status: <number> }
        const payload = { status: statusValue };

        const response = await axios.patch(
            `${API_BASE}/${orderId}/item/${itemId}`,
            payload,
            { headers: { 'Content-Type': 'application/json' } }
        );

        return response.data;
    } catch (error) {
        console.error(`Error al actualizar el item ${itemId} de la orden ${orderId}:`, error.response?.data || error.message);
        throw error;
    }
}

export async function cancelOrder(orderId) {
    
    const CLOSED_STATUS_ID = 5;
    console.log(`Cerrando/Cancelando orden ${orderId} (estado ${CLOSED_STATUS_ID})...`);

    try {
        // 1Obtenemos la orden completa (con sus ítems)
        const response = await axios.get(`${API_BASE}/${orderId}`);
        const order = response.data;

        if (!order.items || order.items.length === 0) {
            console.warn(`La orden ${orderId} no tiene ítems para cancelar.`);
            return;
        }

        //  Recorremos los ítems y los actualizamos uno por uno
        for (const item of order.items) {
            console.log(`→ Cancelando ítem ${item.id}...`);
            await updateOrderItemStatus(orderId, item.id, { status: CLOSED_STATUS_ID });
        }

        console.log(` Orden ${orderId} cancelada correctamente (todos los ítems en estado 5).`);
    } catch (error) {
        console.error(`Error al cancelar la orden ${orderId}:`, error.response?.data || error.message);
        throw error;
    }
}

export async function deliverOrder(orderId) {
    const DELIVERED_STATUS_ID = 4;
    console.log(`Marcando orden ${orderId} como ENTREGADA (estado ${DELIVERED_STATUS_ID})...`);

    try {
        // 1️⃣ Obtener la orden con sus ítems
        const response = await axios.get(`${API_BASE}/${orderId}`);
        const order = response.data;

        if (!order.items || order.items.length === 0) {
            console.warn(`⚠️ La orden ${orderId} no tiene ítems para marcar como entregados.`);
            return;
        }

        // 2️⃣ Recorremos los ítems y actualizamos a estado 4
        for (const item of order.items) {
            console.log(`→ Marcando ítem ${item.id} como entregado...`);
            await updateOrderItemStatus(orderId, item.id, { status: DELIVERED_STATUS_ID });
        }

        console.log(`✅ Orden ${orderId} marcada como ENTREGADA (todos los ítems en estado ${DELIVERED_STATUS_ID}).`);
    } catch (error) {
        console.error(`Error al marcar la orden ${orderId} como entregada:`, error.response?.data || error.message);
        throw error;
    }
}