using Application.Exceptions;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Models.Request;
using Application.Models.Response.Order;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;
using Microsoft.AspNetCore.Http;
using Application.Interfaces.IStatus.Repository;

namespace Application.Services.OrderService
{
    public class UpdateOrderItemStatusUseCase : IUpdateOrderItemStatusUseCase
    {
        private readonly IOrderCommand _orderCommand;
        private readonly IOrderQuery _orderQuery;
        private readonly IStatusQuery _statusQuery;

        public UpdateOrderItemStatusUseCase(IOrderCommand orderCommand, IOrderQuery orderQuery, IStatusQuery statusQuery)
        {
            _orderQuery = orderQuery;
            _orderCommand = orderCommand;
            _statusQuery = statusQuery;
        }

        public async Task<OrderUpdateReponse> UpdateItemStatus(long orderId, int itemId, OrderItemUpdateRequest request)
        {
            // 1. Buscar la orden
            var order = await _orderQuery.GetOrderById(orderId);
            if (order == null)
                throw new NotFoundException("Order not found");

            // 2. Buscar el item
            var item = order.OrderItems.FirstOrDefault(i => i.OrderItemId == itemId);
            if (item == null)
                throw new NotFoundException("Item not found in the order");

            // 3. Validar transición de estado (opcional, según reglas de negocio)
            if (!IsValidTransition(item.StatusId, request.status))
                throw new BadHttpRequestException("Invalid status transition");

            var newStatus = await _statusQuery.GetStatusById(request.status);
            if (newStatus == null)
                throw new NotFoundException("Status not found");

            // 4. Actualizar estado del ítem
            item.StatusId = request.status;
            item.Status = newStatus;
            // 5. Recalcular estado de la orden
            UpdateOrderStatus(order);

            // 6. Guardar cambios
            await _orderCommand.UpdateOrder(order);

            // 7. Respuesta
            return new OrderUpdateReponse
            {
                orderNumber = (int)order.OrderId,
                totalAmount = (double)order.Price,
                UpdateAt = DateTime.UtcNow
            };
        }

        private void UpdateOrderStatus(Order order)
        {
            if (order.OrderItems.All(i => i.StatusId == (int)OrderStatus.Closed))
                order.StatusId = (int)OrderStatus.Closed;
            else if (order.OrderItems.All(i => i.StatusId == (int)OrderStatus.Ready))
                order.StatusId = (int)OrderStatus.Ready;
            else if (order.OrderItems.Any(i => i.StatusId == (int)OrderStatus.InProgress))
                order.StatusId = (int)OrderStatus.InProgress;
            else if (order.OrderItems.Any(i => i.StatusId == (int)OrderStatus.Delivery))
                order.StatusId = (int)OrderStatus.Delivery;
            else
                order.StatusId = (int)OrderStatus.Pending;
        }

        private bool IsValidTransition(int current, int next)
        {
            // Ejemplo de reglas básicas: Pendiente -> En preparación -> Listo -> Entregado
            if (current == (int)OrderStatus.Closed && next != (int)OrderStatus.Closed)
                return false; // no se puede reabrir
            if (current == (int)OrderStatus.Delivery && next == (int)OrderStatus.InProgress)
                return false; // no volver atrás
            return true;
        }

    }

}
