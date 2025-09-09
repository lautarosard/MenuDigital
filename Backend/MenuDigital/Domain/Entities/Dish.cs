using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Dish
    {
        public Guid DishId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        //FK CATEGORY
        public int Category { get; set; }
        public Category CategoryEnt { get; set; }

        //RELATIONSHIP ORDERITEM
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
