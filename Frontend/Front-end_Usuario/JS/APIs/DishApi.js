const API_BASE = "https://localhost:7280/api/v1/Dish";

export async function getDishes() {
    try {
        const response = await axios.get(API_BASE);
        if(response.data && Array.isArray(response.data)) 
        {
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

export async function getDishById(id) {
    try {
        const response = await axios.get(`${API_BASE}/${id}`);
        return response.data;
    } catch (error) {
        console.error("Error al obtener el plato:", error);
        return null;
    }
}

export async function createDish(dish) {
    try {
        const response = await axios.post(API_BASE, dish);
        return response.data;
    } catch (error) {
        console.error("Error al crear el plato:", error);
        return null;
    }
}

export async function UpdateDish(id, dishUpdated) {
    try {
        const response = await axios.put(`${API_BASE}/${id}`, dishUpdated,{
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

export async function deleteDish(id) {
    try {
        const response = await axios.delete(`${API_BASE}/${id}`);
        return response.data;
    } catch (error) {
        console.error("Error al eliminar el plato:", error);
        return null;
    }
}
