using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IDish.Repository
{
    public interface IDishCommand
    {
        Task InsertDish(Dish dish);
        Task UpdateDish(Dish dish);
        Task RemoveDish(Dish dish);
    }
}
