using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IDish
{
    public interface IDishQuery
    {
        Task<Dish?> GetDishById(Guid id);
        Task<List<Dish>> GetAllDishes();
    }
}
