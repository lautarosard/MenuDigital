using Application.Models.Request;
using Application.Models.Response.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder
{
    public interface IUpdateItemFromOrder
    {
        public Task<OrderUpdateReponse> UpdateItemQuantity(long orderId, OrderUpdateRequest listItems);
    }
}
