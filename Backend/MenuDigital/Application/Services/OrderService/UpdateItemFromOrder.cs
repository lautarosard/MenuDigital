using Application.Exceptions;
using Application.Interfaces.IDeliveryType.Repository;
using Application.Interfaces.IDish.Repository;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.Repository;
using Application.Interfaces.IOrderItem.Repository;
using Application.Interfaces.IStatus.Repository;
using Application.Models.Request;
using Application.Models.Response.Order;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.OrderService
{
    public class UpdateItemFromOrder : IUpdateItemFromOrderUseCase
    {
        private readonly IOrderCommand _orderCommand;
        private readonly IOrderQuery _orderQuery;
        private readonly IOrderItemCommand _orderItemCommand;
        private readonly IOrderItemQuery _orderItemQuery;
        private readonly IDishQuery _dishQuery;

        public UpdateItemFromOrder(
            IOrderQuery orderQuery,
            IOrderCommand orderCommand,
            IDishQuery dishQuery,
            IOrderItemCommand orderItemCommand,
            IOrderItemQuery orderItemQuery)
        {
            _orderCommand = orderCommand;
            _orderQuery = orderQuery;
            _dishQuery = dishQuery;
            _orderItemCommand = orderItemCommand;
            _orderItemQuery = orderItemQuery;
        }

        public async Task<OrderUpdateReponse> UpdateItemQuantity(long orderId, OrderUpdateRequest request)
        {
            // 1️ Buscar la orden
            var order = await _orderQuery.GetOrderById(orderId);
            if (order == null)
                throw new NotFoundException($"Order with ID {orderId} not found.");

            // 2️ Validar estado (solo Pending = 1 modificable)
            if (order.OverallStatus.Id != 1)
                throw new BadHttpRequestException("No se puede modificar una orden que ya está en preparación.");

            // 3️ Validar que la lista de items no esté vacía ni con cantidades negativas
            if (request.items == null || !request.items.Any())
                throw new BadHttpRequestException("La orden debe contener al menos un ítem.");

            if (request.items.Any(i => i.quantity < 0))
                throw new BadHttpRequestException("La cantidad de cada ítem debe ser igual o mayor a 0.");

            // 4️ Obtener todos los platos en una sola query
            var dishIds = request.items.Select(i => i.id).Distinct().ToList();
            var dishesFromDb = await _dishQuery.GetDishesByIds(dishIds);
            var dishDictionary = dishesFromDb.ToDictionary(d => d.DishId);

            if (dishesFromDb.Count != dishIds.Count)
                throw new BadHttpRequestException("Uno o más platos especificados no existen.");

            if (dishesFromDb.Any(d => !d.Available))
                throw new BadHttpRequestException("Uno o más platos especificados no están disponibles.");

            // 5️ Lógica PATCH (Agregar / Actualizar / Borrar)
            var itemsToRemove = new List<OrderItem>();

            foreach (var itemReq in request.items)
            {
                var existingItem = order.OrderItems.FirstOrDefault(i => i.DishId == itemReq.id);

                if (itemReq.quantity > 0)
                {
                    // Agregar o actualizar
                    if (existingItem != null)
                    {
                        existingItem.Quantity = itemReq.quantity;
                        existingItem.Notes = itemReq.notes;
                        // EF Core rastrea como 'Modified'
                    }
                    else
                    {
                        order.OrderItems.Add(new OrderItem
                        {
                            DishId = itemReq.id,
                            Quantity = itemReq.quantity,
                            Notes = itemReq.notes,
                            StatusId = 1, // Pending
                            OrderId = order.OrderId
                        });
                    }
                }
                else if (existingItem != null)
                {
                    // Borrar si quantity == 0
                    itemsToRemove.Add(existingItem);
                }
            }

            // 6️ Borrar ítems de la orden
            if (itemsToRemove.Any())
            {
                await _orderItemCommand.RemoveOrderItem(itemsToRemove);
                foreach (var item in itemsToRemove)
                    order.OrderItems.Remove(item);
            }

            // 7️ Si la orden quedó vacía, marcarla como cerrada
            if (!order.OrderItems.Any())
                order.StatusId = 5; // Closed

            // 8 Recalcular el precio
            order.Price = await CalculateTotalPrice(order.OrderItems.ToList());
            order.UpdateDate = DateTime.UtcNow;

            // 9 Guardar cambios
            await _orderCommand.UpdateOrder(order);

            // Retornar respuesta
            return new OrderUpdateReponse
            {
                orderNumber = (int)order.OrderId,
                totalAmount = (double)order.Price,
                UpdateAt = order.UpdateDate
            };
        }

        // Recalcular precio optimizado
        private async Task<decimal> CalculateTotalPrice(List<OrderItem> orderItems)
        {
            if (!orderItems.Any())
                return 0;

            var dishIds = orderItems.Select(i => i.DishId).Distinct().ToList();
            var dishes = await _dishQuery.GetDishesByIds(dishIds);
            var dishDict = dishes.ToDictionary(d => d.DishId);

            decimal total = 0;
            foreach (var item in orderItems)
            {
                if (dishDict.TryGetValue(item.DishId, out var dish))
                    total += dish.Price * item.Quantity;
                else
                    throw new NotFoundException($"No se encontró el plato con ID {item.DishId} al recalcular el total.");
            }
            return total;
        }
    }
}
