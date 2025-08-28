using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IOrderItem
{
    public interface IOrderItemCommand
    {
        Task InsertOrderItem(OrderItem orderItem);
        Task UpdateOrderItem(OrderItem orderItem);
        Task RemoveOrderItem(OrderItem orderItem);
    }
}
