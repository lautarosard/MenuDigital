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

namespace Application.Services.OrderService
{
    public class UpdateOrderItemStatusUseCase : IUpdateOrderItemStatusUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderItemStatusUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderUpdateReponse> UpdateItemStatus(long orderId, int itemId, OrderItemUpdateRequest request)
        {
            // 1. Buscar la orden
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
                throw new NotFoundException("Order not found");

            // 2. Buscar el item
            var item = order.OrderItems.FirstOrDefault(i => i.OrderItemId == itemId);
            if (item == null)
                throw new NotFoundException("Item not found in the order");

            // 3. Validar transición de estado (opcional, según reglas de negocio)
            if (!IsValidTransition(item.StatusId, request.status))
                throw new BadHttpRequestException("Invalid status transition");

            // 4. Actualizar estado del ítem
            item.StatusId = request.status;

            // 5. Recalcular estado de la orden
            UpdateOrderStatus(order);

            // 6. Guardar cambios
            await _orderRepository.UpdateOrder(order);

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
