// =================================================
// Archivo: Assets/JS/main.js
// Responsabilidad: Punto de entrada de la aplicación.
// =================================================

// 1. Importamos la función principal del módulo que maneja la página de productos.
// Le podemos dar cualquier nombre, pero "inicializarPaginaProductos" es claro.
import inicializar from './Pages/PlatosAdmin.js';

// 2. Esperamos a que el navegador haya cargado y analizado todo el HTML.
// Es una buena práctica para evitar errores si el script intentara
// manipular elementos que aún no existen.
document.addEventListener('DOMContentLoaded', () => {
    
    console.log("DOM listo. Lanzando la aplicación desde main.js...");

    // 3. Le damos la orden al módulo importado para que comience su trabajo.
    inicializar();

});