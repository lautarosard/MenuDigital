// 1. Importamos las funciones que necesitamos
import { getDishes } from './../APIs/DishApi.js'; 
import { renderCardPlato } from './../Components/Platos/renderCardPlato.js'; 
// 2. Obtenemos la referencia al contenedor
const contenedorPlatos = document.getElementById('platos-container');

async function inicializar() {
    console.log("Inicializando página de platos...");
    try {
        contenedorPlatos.innerHTML = '<p class="text-center">Cargando menú...</p>';
        const platos = await getDishes();
        contenedorPlatos.innerHTML = '';

        if (platos.length === 0) {
            contenedorPlatos.innerHTML = '<p class="text-center">No hay platos disponibles.</p>';
            return;
        }
        // 1. Creamos nuestra "bandeja" vacía
        const fragment = document.createDocumentFragment();

        // 2. Recorremos los platos y los agregamos A LA BANDEJA
        platos.forEach(plato => {
            const tarjetaElemento = renderCardPlato(plato);
            fragment.appendChild(tarjetaElemento);
        });

        // 3. Hacemos UNA SOLA operación en el DOM para agregar todo
        contenedorPlatos.appendChild(fragment);

    } catch (error) {
        console.error("Error al inicializar la página:", error);
        contenedorPlatos.innerHTML = '<p class="text-center text-danger">No se pudo cargar el menú. Intente más tarde.</p>';
    }
}

// Exportamos la función para que main.js la llame
export default inicializar;