import inicializarGestionMenu from './Pages/GestionMenuAdmin.js';
import inicializarPanelInicio from './Pages/PanelInicioAdmin.js';
document.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname;
    // 2. Añadir la nueva ruta
    if (path.includes('Gest_Menu.html')) {
        inicializarGestionMenu();
    }
    else if (path.includes('index.html')) {
        inicializarPanelInicio();
    }
});