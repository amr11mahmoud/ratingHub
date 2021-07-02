﻿using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.ProductReview
{
    public class ProductReview:Entity
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public int ProductId { get; set; }
    }
}
