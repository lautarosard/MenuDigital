﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        //RELATIONSHIP DISH
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();

    }
}
