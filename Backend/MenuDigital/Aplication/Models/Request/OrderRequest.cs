using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request
{
    public class OrderRequest
    {
        public string DeliveryTo { get; set; }
        public string Notes { get; set; }
        public int Price { get; set; }

        //Dates dude (see if i need to add them here)
    }
}
