using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Models.Response;
using Application.Enums;

namespace Application.Interfaces.IDish.Repository
{
    public interface IDishQuery
    {
        Task<Dish?> GetDishById(Guid id);
        Task<IEnumerable<Dish>> GetAllAsync(
            string? name = null,
            int? categoryId = null,
            OrderPrice? priceOrder = OrderPrice.ASC,
            bool? onlyActive = true
            );

        Task<bool> DishExists(string name, Guid? id);
    }
}
