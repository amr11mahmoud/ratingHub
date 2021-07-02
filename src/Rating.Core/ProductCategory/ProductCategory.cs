using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.ProductCategory
{
    public class ProductCategory:Entity
    {
        public string Name { get; set; }
        public string LocalName { get; set; }
        public int? ParentCategId { get; set; }
    }
}
