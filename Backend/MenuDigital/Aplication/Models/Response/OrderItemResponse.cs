using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Response
{
    public class OrderItemResponse
    {
        public long Id { get; set; }

        public int Quantity { get; set; }
        public string Notes { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
