using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //RELATIONSHIP ORDERITEM
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        //RELATIONSHIP ORDER
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
