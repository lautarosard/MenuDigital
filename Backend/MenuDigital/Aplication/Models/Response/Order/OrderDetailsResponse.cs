using Application.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Response.Order
{
    public class OrderDetailsResponse
    {
        public int orderNumber { get; set; }
        public double totalAmount { get; set; }
        public string deliveryTo { get; set; }
        public string notes { get; set; }
        public GenericResponse status { get; set; }
        public GenericResponse deliveryType { get; set; }
        public List<Items> items { get; set; }

        public DateTime createAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
