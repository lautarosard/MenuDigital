using Application.Interfaces.IDish.Repository;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Interfaces.IOrderItem.Repository;
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
    public class UpdateItemFromOrder : IUpdateItemFromOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDishRepository _dishRepository;
        public UpdateItemFromOrder(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IDishRepository dishRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _dishRepository = dishRepository;
        }
        public async Task<OrderUpdateReponse> UpdateItemQuantity(long orderId, OrderUpdateRequest listItems)
        {
            //1. buscar la orden por id
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                throw new Exception(""); 
            }

            //2. no se puede modificar si no está 'Pending'
            if (order.OverallStatus.Id != 1) 
            {
                throw new Exception("");
            }
            //3. borrar todos los items de la orden
            await _orderItemRepository.RemoveOrderItem(order.OrderItems);
            //4. crear los nuevos items
            var newOrderItems = listItems.items.Select(item => new OrderItem
            {
                OrderId = orderId,
                DishId = item.id,
                Quantity = item.quantity,
                Notes = item.notes,
                StatusId = 1
            }).ToList();
            //5. Insertar los nuevos items
            await _orderItemRepository.InsertOrderItemRange(newOrderItems);
            //6. recalcular el precio total
            decimal totalPrice = 0;
            foreach (var item in newOrderItems)
            {
                var dish = await _dishRepository.GetDishById(item.DishId);
                if (dish != null)
                {
                    totalPrice += dish.Price * item.Quantity;
                }
            }
            //7. actualizar la orden
            order.Price = totalPrice;
            order.UpdateDate = DateTime.Now;
            await _orderRepository.UpdateOrder(order);
            //8. retornar la respuesta
            return new OrderUpdateReponse
            {
                orderNumber = (int)order.OrderId,
                totalAmount = (double)order.Price,
                UpdateAt = order.UpdateDate
            };
        }
    }
}
