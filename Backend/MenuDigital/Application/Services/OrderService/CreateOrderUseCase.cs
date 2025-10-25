using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces.IDeliveryType.Repository;
using Application.Interfaces.IDish.Repository;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Interfaces.IOrderItem.Repository;
using Application.Models.Request;
using Application.Models.Response.Order;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services.OrderService
{
    public class CreateOrderUseCase : ICreateOrderUseCase
    {
        private readonly IOrderCommand _orderCommand;
        private readonly IOrderQuery _orderQuery;
        private readonly IDeliveryTypeQuery _deliveryTypeQuery;
        private readonly IDishCommand _dishCommand;
        private readonly IDishQuery _dishQuery;
        private readonly IOrderItemCommand _orderItemCommand;
        private readonly IOrderItemQuery _orderItemQuery;
        public CreateOrderUseCase(
            IOrderQuery orderQuery,
            IOrderCommand orderCommand,
            IDeliveryTypeQuery deliveryTypeQuery,
            IDishCommand dishCommand,
            IDishQuery dishQuery,
            IOrderItemCommand orderItemCommand,
            IOrderItemQuery orderItemQuery
            )
        {
            _orderCommand = orderCommand;
            _orderQuery = orderQuery;
            _deliveryTypeQuery = deliveryTypeQuery;
            _dishCommand = dishCommand;
            _dishQuery = dishQuery;
            _orderItemCommand = orderItemCommand;
            _orderItemQuery = orderItemQuery;
        }
        public async Task<OrderCreateResponse?> CreateOrder(OrderRequest orderRequest)
        {
            // Validar existencia de items
            if (orderRequest.items == null || !orderRequest.items.Any())
                throw new BadHttpRequestException("Debe especificar al menos un item en la orden.");

            // Validar cantidades > 0
            foreach (var item in orderRequest.items)
            {
                if (item.quantity <= 0)
                    throw new BadHttpRequestException("La cantidad de cada item debe ser mayor a 0.");
            }

            // Validar tipo de entrega válido
            if (orderRequest.delivery == null || orderRequest.delivery.id <= 0)
                throw new BadHttpRequestException("Debe especificar un tipo de entrega válido.");

            // Obtener tipo de entrega
            var deliveryType = await _deliveryTypeQuery.GetDeliveryTypeById(orderRequest.delivery.id);
            if (deliveryType == null)
                throw new NotFoundException("Tipo de entrega no existe");

            //crear order
            
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
            await _orderCommand.InsertOrder(order);
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
            Console.WriteLine("OrderItems:");
            foreach (var oi in order.OrderItems)
            {
                Console.WriteLine($"ItemID: {oi.OrderItemId}, DishID: {oi.DishId}, Notes: {oi.Notes}");
            }
            order.Price = await CalculateTotalPrice(listItems);
            await _orderItemCommand.InsertOrderItemRange(listorderItems);
            await _orderCommand.UpdateOrder(order);
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
                var dish = await _dishQuery.GetDishById(item.id);
                if (dish == null)
                {
                    throw new NotFoundException($"Dish with ID {item.id} not found.");
                }
                total += dish.Price * item.quantity;
            }
            return total;
        }
    }
}
