using Application.Interfaces.IDeliveryType.Repository;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Interfaces.IStatus.Repository;
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
    public class GetOrderWithFilterUseCase : IGetOrderWithFilterUseCase
    {
        private readonly IOrderCommand _orderCommand;
        private readonly IOrderQuery _orderQuery;
        private readonly IStatusQuery _statusQuery;
        private readonly IDeliveryTypeQuery _deliveryTypeQuery;
        public GetOrderWithFilterUseCase(
            IOrderQuery orderQuery,
            IOrderCommand orderCommand,
            IStatusQuery statusQuery,
            IDeliveryTypeQuery deliveryTypeQuery
            )
        {
            _orderQuery = orderQuery;
            _orderCommand = orderCommand;
            _statusQuery = statusQuery;
            _deliveryTypeQuery = deliveryTypeQuery;
        }
        public async Task<IEnumerable<OrderDetailsResponse?>> GetOrderWithFilter(int? statusId, DateTime? from, DateTime? to)
        {
            var orders = await _orderQuery.GetOrderWithFilter(statusId, from, to);
            
            if (orders == null || !orders.Any())
            {
                return Enumerable.Empty<OrderDetailsResponse?>();
            }

            var orderResponses = orders.Select(order =>
            new OrderDetailsResponse
            {
                orderNumber = (int)order.OrderId,
                totalAmount = (double)order.Price,
                deliveryTo = order.DeliveryTo,
                notes = order.Notes,
                status = new GenericResponse { Id = order.StatusId, Name = order.OverallStatus?.Name ?? "Desconocido"},
                deliveryType = new GenericResponse { Id = order.DeliveryTypeId, Name= order.DeliveryType?.Name ?? "Desconocido"},
                items = order.OrderItems.Select(item => new OrderItemResponse
                {
                    Id = (int)item.OrderItemId,
                    Quantity = item.Quantity,
                    notes = item.Dish?.Name,
                    dish = new DishShortResponse { Id = item.DishId, Name = item.Dish?.Name ?? "Desconocido", Image = item.Dish?.ImageUrl ?? "No encontrada"},
                    status = new GenericResponse { Id = item.Status.Id, Name = item.Status?.Name ?? "Desconocido" }
                }).ToList(),
                createAt = order.CreateDate,
                UpdateAt = order.UpdateDate
            });
            return orderResponses;
        }
    }
}
