const API_BASE = "https://localhost:7280/api/v1/Category";

export async function getCategories() {
    try {
        const response = await axios.get(API_BASE);
        if (response.data && Array.isArray(response.data)) {
            return response.data;
        } else {
            console.warn("No se encontraron categorías en la respuesta de la API.");
            return [];
        }
    } catch (error) {
        console.error("Error al obtener las categorías:", error);
        return [];
    }
}