using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder.Repository
{
    public interface IOrderRepository
    {
        //Queries
        Task<Order?> GetOrderById(long id);
        Task<List<Order>> GetAllOrders();
        //Commands
        Task InsertOrder(Order order);
        Task UpdateOrder(Order order);
        Task RemoveOrder(Order order);
    }
}
