using Application.Models.Response.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder
{
    public interface IGetOrderWithFilterUseCase
    {
        Task<IEnumerable<OrderDetailsResponse?>> GetOrderWithFilter(int? statusId, DateTime? from, DateTime? to);
    }
}
