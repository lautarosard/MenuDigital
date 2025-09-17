using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Response.Dish
{
    public class DishResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public GenericResponse Category { get; set; }
        public bool isActive { get; set; }
        public string ImageUrl { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updateAt { get; set; }
    }
}
