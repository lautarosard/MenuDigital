using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Response;
using Application.Models.Request;

namespace Application.Interfaces.IOrderItem
{
    public interface IOrderItemService
    {
        // List
        Task<List<OrderItemResponse>> GetAllOrderItems();

        // Create
        Task<OrderItemResponse> CreateOrderItem(OrderItemUpdateRequest orderItemRequest);

        // Update
        Task<OrderItemResponse> UpdateOrderItem(int id, OrderItemUpdateRequest orderItemRequest);

        // Delete
        Task<OrderItemResponse> DeleteOrderItem(int id);

        // Queries
        Task<OrderItemResponse> GetOrderItemById(int id);
    }
}
