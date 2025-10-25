// 1. Importamos la función principal del módulo que maneja la página de productos.
// Le podemos dar cualquier nombre, pero "inicializarPaginaProductos" es claro.
import inicializar from './Pages/DishAdmin.js';
import inicializarMisOrdenes from './Pages/MisOrdenesAdmin.js';
// 2. Esperamos a que el navegador haya cargado y analizado todo el HTML.
// Es una buena práctica para evitar errores si el script intentara
// manipular elementos que aún no existen.
document.addEventListener('DOMContentLoaded', () => {
    
    console.log("DOM listo. Lanzando la aplicación desde main.js...");
    const path = window.location.pathname;

    if (path.includes('MisOrdenes.html')) { // <-- Esta es tu línea 12 (o similar)
        inicializarMisOrdenes();
    } else if (path.includes('index.html') || path === '/') {
        inicializar();
    }

});