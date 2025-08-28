using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request
{
    public class OrderItemRequest
    {
        public int Quantity { get; set; }
        public string Notes { get; set; }

        //see the same thing of the DishRequest {dates}
    }
}
