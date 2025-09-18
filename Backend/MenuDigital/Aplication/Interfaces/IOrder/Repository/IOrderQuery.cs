using Application.Models.Response.Order;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder.Repository
{
    public interface IOrderQuery
    {
        Task<Order?> GetOrderById(long id);
        Task<IEnumerable<Order?>> GetOrderWithFilter(int? statusId, DateTime? from, DateTime? to);

        Task<List<Order>> GetAllOrders();
    }
}
