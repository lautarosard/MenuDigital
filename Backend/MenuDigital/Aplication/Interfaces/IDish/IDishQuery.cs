using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Models.Response;
using Application.Enums;

namespace Application.Interfaces.IDish
{
    public interface IDishQuery
    {
        Task<Dish?> GetDishById(Guid id);
        Task<List<Dish>> GetAllDishes();
        Task<IEnumerable<Dish>> GetAllAsync(string? name = null, int? categoryId = null, OrderPrice? priceOrder = OrderPrice.ASC);

        Task<bool> DishExists(string name);
        //Task<IReadOnlyList<Domain.Entities.Dish>> SearchAsync(string? name, int? categoryId, string? priceOrder);
    }
}
