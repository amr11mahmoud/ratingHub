using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.ProductCategory.Dto
{
    [AutoMap(typeof(ProductCategory))]
    public class ProductCategoryDto:EntityDto
    {
        public string Name { get; set; }
        public int? ParentCategId { get; set; }
    }
}
