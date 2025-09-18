using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IOrder.Repository;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderCommand _orderCommand;
        private readonly IOrderQuery _orderQuery;
        public OrderRepository(IOrderCommand orderCommand, IOrderQuery orderQuery)
        {
            _orderCommand = orderCommand;
            _orderQuery = orderQuery;
        }
        //Queries
        public Task<Order?> GetOrderById(long id)
        {
            return _orderQuery.GetOrderById(id);
        }
        public Task<List<Order>> GetAllOrders()
        {
            return _orderQuery.GetAllOrders();
        }
        public Task<IEnumerable<Order?>> GetOrderWithFilter(int? statusId, DateTime? from, DateTime? to)
        {
            return _orderQuery.GetOrderWithFilter(statusId, from, to);
        }
        //Commands
        public Task InsertOrder(Order order)
        {
            return _orderCommand.InsertOrder(order);
        }
        public Task UpdateOrder(Order order)
        {
            return _orderCommand.UpdateOrder(order);
        }
        public Task RemoveOrder(Order order)
        {
            return _orderCommand.RemoveOrder(order);
        }
    }
}
