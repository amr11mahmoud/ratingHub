using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.ProductFeature
{
    public class ProductFeature:Entity
    {
        public string Name { get; set; }
        public string LocalName { get; set; }
        public string Value { get; set; }
        public string LocalValue { get; set; }

        public ICollection<ProductProductFeature.ProductProductFeature> ProductProductFeature { get; set; }

    }
}
