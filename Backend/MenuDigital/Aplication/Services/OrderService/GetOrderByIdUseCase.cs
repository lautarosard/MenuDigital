using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Models.Response;
using Application.Models.Response.Dish;
using Application.Models.Response.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.OrderService
{
    public class GetOrderByIdUseCase : IGetOrderByIdUseCase
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrderByIdUseCase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<OrderDetailsResponse?> GetOrderById(long id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order != null)
            {
                var orderDetails = new OrderDetailsResponse
                {
                    orderNumber = (int)order.OrderId,
                    totalAmount = (double)order.Price,
                    deliveryTo = order.DeliveryTo,
                    notes = order.Notes,
                    status = new GenericResponse { Id = order.StatusId, Name = order.OverallStatus?.Name ?? "Desconocido" },
                    deliveryType = new GenericResponse { Id = order.DeliveryTypeId, Name = order.DeliveryType?.Name ?? "Desconocido" },
                    items = order.OrderItems.Select(item => new OrderItemResponse
                    {
                        Id = 2,
                        Quantity = item.Quantity,
                        notes = item.Dish?.Name,
                        dish = new DishShortResponse { Id = item.DishId, Name = item.Dish?.Name ?? "Desconocido", Image = item.Dish?.ImageUrl ?? "No encontrada" },
                        status = new GenericResponse { Id = item.Status.Id, Name = item.Status?.Name ?? "Desconocido" }
                    }).ToList(),
                    createAt = order.CreateDate,
                    UpdateAt = order.UpdateDate
                };
            return orderDetails;
            };
            return null;
        }
    }
}
