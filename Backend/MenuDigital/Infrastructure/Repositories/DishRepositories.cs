using Application.Enums;
using Application.Interfaces.IDish.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DishRepositories : IDishRepository
    {
        private readonly IDishQuery _dishQuery;
        private readonly IDishCommand _dishCommand;
        public DishRepositories(IDishQuery dishQuery, IDishCommand dishCommand)
        {
            _dishQuery = dishQuery;
            _dishCommand = dishCommand;
        }
        //Queries
        public Task<Dish?> GetDishById(Guid id)
        {
            return _dishQuery.GetDishById(id);
        }
        public Task<IEnumerable<Dish>> GetAllAsync(
            string? name = null, 
            int? categoryId = null, 
            OrderPrice? priceOrder = OrderPrice.ASC, 
            bool? onlyActive = true
            )
        {
            return _dishQuery.GetAllAsync(name, categoryId, priceOrder, onlyActive);
        }
        public Task<bool> DishExists(string name, Guid? id)
        {
            return _dishQuery.DishExists(name, id);
        }
        //Commands
        public Task InsertDish(Dish dish)
        {
            return _dishCommand.InsertDish(dish);
        }
        public Task UpdateDish(Dish dish)
        {
            return _dishCommand.UpdateDish(dish);
        }
        public Task RemoveDish(Dish dish)
        {
            return _dishCommand.RemoveDish(dish);
        }
    }
}
