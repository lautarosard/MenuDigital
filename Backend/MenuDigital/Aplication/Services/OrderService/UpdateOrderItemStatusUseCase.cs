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

            // 2. Buscar el item dentro de la orden
            var item = order.OrderItems.FirstOrDefault(i => i.OrderItemId == itemId);
            if (item == null)
                throw new NotFoundException("Item not found in the order");

            // 3. Actualizar estado del ítem
            item.StatusId = request.status;

            // 4. Actualizar estado general de la orden
            UpdateOrderStatus(order);

            // 5. Persistir cambios
            await _orderRepository.UpdateOrder(order);

            // 6. Devolver respuesta
            return new OrderUpdateReponse
            {
                orderNumber = (int)order.OrderId,                
                totalAmount = (double)order.Price,     
                UpdateAt = DateTime.UtcNow             
            };
        }

        private void UpdateOrderStatus(Order order)
        {
            // Si todos los ítems están Ready -> orden Ready
            if (order.OrderItems.All(i => i.StatusId == 3))
            {
                order.StatusId = 3;
            }
            // Si al menos uno está In Progress -> orden In Progress
            else if (order.OrderItems.Any(i => i.StatusId == 2))
            {
                order.StatusId = 2;
            }
            // Si al menos uno está Delivery -> orden Delivery
            else if (order.OrderItems.Any(i => i.StatusId == 4))
            {
                order.StatusId = 4;
            }
            // Si todos los ítems están Closed -> orden Closed
            else if (order.OrderItems.All(i => i.StatusId == 5))
            {
                order.StatusId = 5;
            }
            // Caso inicial -> Pending
            else
            {
                order.StatusId = 1;
            }
        }
    }

}
