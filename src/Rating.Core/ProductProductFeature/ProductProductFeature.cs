using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.ProductProductFeature
{
    public class ProductProductFeature:Entity
    {
        public int ProductId { get; set; }
        public int FeatureId { get; set; }

        public ProductFeature.ProductFeature ProductFeature { get; set; }
    }
}
