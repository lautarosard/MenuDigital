// En: JS/Components/Orders/renderOrderCard.js

/**
 * Mapea los estados de la API a texto y colores de Bootstrap
 */
export const statusConfig = {
    'PENDING': { text: 'Pendiente', color: 'warning' },
    'READY': { text: 'Ready', color: 'success' },
    'DELIVERED': { text: 'Entregado', color: 'secondary' },
    'CANCELLED': { text: 'Cancelado', color: 'danger' },
    'DEFAULT': { text: 'Desconocido', color: 'dark' }
};
// Helper: normaliza un string para hacer el lookup
function normalizeStatusName(name) {
    if (!name) return '';
    return name.toString().toUpperCase().trim();
}

// Helper: obtener configuración (color/text) a partir del estado
function getStatusConfig(status) {
    // status puede ser { id, name } o string
    const rawName = (typeof status === 'string') ? status : (status && status.name) ? status.name : '';
    const key = normalizeStatusName(rawName);

    // Intentamos buscar en statusConfig con la clave normalizada
    const cfg = statusConfig[key];
    if (cfg) return cfg;

    // Si no hay configuración, devolvemos un fallback legible
    const fallbackText = rawName || 'DESCONOCIDO';
    return { text: fallbackText, color: 'secondary' }; // color 'secondary' para fallback gris
}
/**
 * Formatea la fecha a un formato legible (ej: 14/10/2025)
 */
// AÑADIMOS "export" para poder reusarlo

/**
 * Crea y devuelve el elemento HTML para una sola tarjeta de pedido.
 */

export function formatLocalDate(dateString) {
    // Si el dateString no existe, devolvemos un texto por defecto
    if (!dateString) {
        return 'Fecha no disponible';
    }
    
    const date = new Date(dateString);

    // Si la fecha que nos pasaron sigue siendo inválida
    if (isNaN(date.getTime())) {
        return 'Fecha inválida';
    }

    // Si todo está bien, la formateamos
    return date.toLocaleDateString('es-AR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
    });
}

export function renderOrderCard(order) {
    
    const cfg = getStatusConfig(order.status);
    const estadoTexto = cfg.text;
    const estadoColor = cfg.color;
    // 1. Obtener la configuración de estado (o usar DEFAULT)
    // Obtenemos la clave del estado (ej: 'PENDING') de forma segura
    const statusKey = (order.status && order.status.name) 
                        ? order.status.name.toUpperCase() 
                        : 'DEFAULT';
                        
    // Usamos la clave para buscar la configuración (ej: { text: 'Pendiente', color: 'warning' })
    const config = statusConfig[statusKey] || statusConfig['DEFAULT'];

    // 2. Crear el contenedor principal (usamos list-group-item de Bootstrap)
    const card = document.createElement('div');
    card.className = 'list-group-item list-group-item-action p-3 shadow-sm border rounded';
    
    // 3. Formatear el total a moneda
    const totalValue = (typeof order.totalAmount === 'number') ? order.totalAmount : 0;

    // Ahora formateamos el valor seguro (totalValue)
    const totalFormatted = totalValue.toLocaleString('es-AR', {
        style: 'currency',
        currency: 'ARS'
    });

    // 4. Definir el HTML interno de la tarjeta
    card.innerHTML = `
        <div class="d-flex w-100 justify-content-between align-items-center">
            
            <div>
                <h5 class="mb-1">Orden #${order.orderNumber}</h5>
                <p class="mb-1"><strong>Total:</strong> ${totalFormatted}</p>
                <small class="text-muted">Realizado el: ${formatLocalDate(order.createAt)}</small>
            </div>

            <div class="text-end">
                <span class="badge bg-${estadoColor} mb-2">${estadoTexto}</span>
                <br>
                <button 
                    class="btn btn-primary btn-sm btn-ver-detalle" 
                    data-order-number="${order.orderNumber}">
                    Ver Detalle / Modificar
                </button>
            </div>

        </div>
    `;

    // 5. Devolver el elemento HTML creado
    return card;
}