using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IDeliveryType.Repository;
using Application.Interfaces.IDish.Repository;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Interfaces.IOrderItem.Repository;
using Application.Models.Request;
using Application.Models.Response.Order;
using Domain.Entities;

namespace Application.Services.OrderService
{
    public class CreateOrderUseCase : ICreateOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDeliveryTypeQuery _deliveryTypeQuery;
        private readonly IDishRepository _dishRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        public CreateOrderUseCase(IOrderRepository orderRepository, 
            IDeliveryTypeQuery deliveryTypeQuery,
            IDishRepository dishRepository,
            IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _deliveryTypeQuery = deliveryTypeQuery;
            _dishRepository = dishRepository;
            _orderItemRepository = orderItemRepository;
        }
        public async Task<OrderCreateResponse?> CreateOrder(OrderRequest orderRequest)
        {

            //crear order
            var deliveryType =  await _deliveryTypeQuery.GetDeliveryTypeById(orderRequest.delivery.id);
            var order = new Order{
                DeliveryTypeId = orderRequest.delivery.id,
                Price = 0, 
                StatusId = 1, 
                DeliveryTo = orderRequest.delivery.to,
                Notes = orderRequest.notes,
                UpdateDate = DateTime.Now,
                CreateDate = DateTime.Now
            };
            //guardar order
            await _orderRepository.InsertOrder(order);
            //crear orderItem
            var listItems = orderRequest.items;
            var listorderItems = listItems.Select(item => new OrderItem
                                {
                                    DishId = item.id,
                                    Quantity = item.quantity,
                                    Notes = item.notes,
                                    StatusId = 1, 
                                    OrderId = order.OrderId,
            }).ToList();
            order.Price = await CalculateTotalPrice(listItems);
            await _orderItemRepository.InsertOrderItemRange(listorderItems);
            await _orderRepository.UpdateOrder(order);
            //relacionar orderItem con dish

            return new OrderCreateResponse
            {
                orderNumber = (int)order.OrderId,// se genera auto, ver
                totalAmount = (double)order.Price,
                createdAt = DateTime.Now
            };

        }
        private async Task<decimal> CalculateTotalPrice(List<Items> orderItems)
        {
            decimal total = 0;
            //dish obtener
            foreach (var item in orderItems)
            {
                var dish = await _dishRepository.GetDishById(item.id);
                total += dish.Price * item.quantity;
            }
            return total;
        }
    }
}
