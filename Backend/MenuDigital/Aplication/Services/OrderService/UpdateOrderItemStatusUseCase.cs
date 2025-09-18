using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Models.Request;
using Application.Models.Response.Order;

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
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            var item = order.OrderItems.FirstOrDefault(i => i.StatusId == itemId);
            if (item == null)
            {
                throw new Exception("Item not found in the order");
            }
            var currentStatus = item.StatusId;
            var newStatus = request.status;
            await _orderRepository.UpdateOrder(order);
            return new OrderUpdateReponse
            {
                orderNumber = order.StatusId,
                totalAmount = (double)order.Price,
                UpdateAt = order.UpdateDate
            };
        }
    }
}
