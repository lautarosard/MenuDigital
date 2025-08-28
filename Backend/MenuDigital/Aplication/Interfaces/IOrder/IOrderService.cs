using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Request;
using Application.Models.Response;


namespace Application.Interfaces.IOrder
{
    public interface IOrderService
    {
        // List
        Task<List<OrderResponse>> GetAllOrders();

        // Create
        Task<OrderResponse> CreateOrder(OrderRequest orderRequest);

        // Update
        Task<OrderResponse> UpdateOrder(int id, OrderRequest orderRequest);

        // Delete
        Task<OrderResponse> DeleteOrder(int id);

        // Queries
        Task<OrderResponse> GetOrderById(int id);

    }
}
