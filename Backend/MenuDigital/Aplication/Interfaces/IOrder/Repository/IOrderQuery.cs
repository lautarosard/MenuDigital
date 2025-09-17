using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IOrder.Repository
{
    public interface IOrderQuery
    {
        Task<Order?> GetOrderById(long id);
        Task<List<Order>> GetAllOrders();
    }
}
