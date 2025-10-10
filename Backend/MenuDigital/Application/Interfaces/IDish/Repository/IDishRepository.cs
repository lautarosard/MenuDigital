using Application.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IDish.Repository
{
    public interface IDishRepository
    {
        //Queries
        Task<Dish?> GetDishById(Guid id);
        Task<IEnumerable<Dish>> GetAllAsync(
            string? name = null,
            int? categoryId = null,
            OrderPrice? priceOrder = OrderPrice.ASC,
            bool? onlyActive = true
        );
        Task<bool> DishExists(string name, Guid? id);

        //Commands
        Task InsertDish(Dish dish);
        Task UpdateDish(Dish dish);
        Task RemoveDish(Dish dish);

    }
}
