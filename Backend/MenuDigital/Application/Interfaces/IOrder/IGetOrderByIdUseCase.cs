using Application.Models.Response.Dish;
using Application.Models.Response.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder
{
    public interface IGetOrderByIdUseCase
    {
        Task<OrderDetailsResponse?> GetOrderById(long id);

    }
}
