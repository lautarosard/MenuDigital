using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IOrderItem.Repository
{
    public interface IOrderItemQuery
    {
        Task<OrderItem?> GetOrderItemById(long id);
        Task<List<OrderItem>> GetAllOrderItems();
    }
}
