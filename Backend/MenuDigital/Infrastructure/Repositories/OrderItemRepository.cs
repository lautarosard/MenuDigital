using Application.Interfaces.IOrderItem;
using Application.Interfaces.IOrderItem.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IOrderItemCommand _orderItemCommand;
        private readonly IOrderItemQuery _orderItemQuery;
        public OrderItemRepository(IOrderItemCommand orderItemCommand, IOrderItemQuery orderItemQuery)
        {
            _orderItemCommand = orderItemCommand;
            _orderItemQuery = orderItemQuery;
        }
        // queries
        public async Task<OrderItem?> GetOrderItemById(long id)
        {
            return await _orderItemQuery.GetOrderItemById(id);
        }
        public async Task<List<OrderItem>> GetAllOrderItems()
        {
            return await _orderItemQuery.GetAllOrderItems();
        }
        public async Task<bool> ExistsByDishId(Guid dishId)
        {
            return await _orderItemQuery.ExistsByDishId(dishId);
        }
        // commands
        public async Task InsertOrderItem(OrderItem orderItem)
        {
            await _orderItemCommand.InsertOrderItem(orderItem);
        }
        public async Task InsertOrderItemRange(List<OrderItem> orderItems)
        {
            await _orderItemCommand.InsertOrderItemRange(orderItems);
        }
        public async Task UpdateOrderItem(OrderItem orderItem)
        {
            await _orderItemCommand.UpdateOrderItem(orderItem);
        }
        public async Task RemoveOrderItem(IEnumerable<OrderItem> orderItem)
        {
            await _orderItemCommand.RemoveOrderItem(orderItem);
        }
    }
}
