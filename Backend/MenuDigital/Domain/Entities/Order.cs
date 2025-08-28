using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        public long OrderId { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime CreateDate { get; set; }
        //FK DELIVERYTYPE
        public int DeliveryTypeId { get; set; }
        public DeliveryType DeliveryType { get; set; }
        //FK STATUS
        public int StatusId { get; set; }
        public Status OverallStatus { get; set; }
        //RELATIONSHIP ORDERITEM
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
