using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Response.Dish
{
    public class UpdateDishResult
    {
        public bool Success { get; set; }
        public bool NotFound { get; set; }
        public bool NameConflict { get; set; }
        public bool DishAvailable { get; set; }
        public DishResponse? UpdatedDish { get; set; }
    }
}
