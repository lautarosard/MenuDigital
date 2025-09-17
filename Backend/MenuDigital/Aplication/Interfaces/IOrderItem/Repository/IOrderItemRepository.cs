using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrderItem.Repository
{
    public interface IOrderItemRepository
    {
        // queries
        Task<OrderItem?> GetOrderItemById(long id);
        Task<List<OrderItem>> GetAllOrderItems();

        // commands
        Task InsertOrderItem(OrderItem orderItem);
        Task InsertOrderItemRange(List<OrderItem> orderItems);
        Task UpdateOrderItem(OrderItem orderItem);
        Task RemoveOrderItem(OrderItem orderItem);

    }
}
