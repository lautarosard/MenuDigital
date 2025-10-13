const API_BASE = "https://localhost:7280/api/v1/Order";

/**
 * Crea una nueva orden.
 * @param {object} orderRequest - El objeto de la orden a crear.
 * @returns {Promise<object>} La respuesta de la orden creada.
 */
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

/**
 * Actualiza los items de una orden existente.
 * @param {number} orderId - El ID de la orden a actualizar.
 * @param {object} orderUpdated - El objeto con los items actualizados.
 * @returns {Promise<object|null>} La respuesta de la actualización o null en caso de error.
 */
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
        return null;
    }
}

/**
 * Actualiza el estado de un item específico en una orden.
 * @param {number} orderId - El ID de la orden.
 * @param {number} itemId - El ID del item dentro de la orden.
 * @param {object} itemUpdate - El objeto con el nuevo estado. Ej: { status: 3 }[cite: 122, 169].
 * @returns {Promise<object|null>} La respuesta de la actualización o null en caso de error.
 */
export async function updateOrderItemStatus(orderId, itemId, itemUpdate) {
    try {
        // CORRECCIÓN: La ruta es /item/ en singular, no /items/
        const response = await axios.patch(`${API_BASE}/${orderId}/item/${itemId}`, itemUpdate, {
            headers: {
                'Content-Type': 'application/json'
            }
        });
        return response.data;
    } catch (error) {
        console.error(`Error al actualizar el item ${itemId} de la orden ${orderId}:`, error.response?.data || error.message);
        return null;
    }
}