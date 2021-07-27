using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Product.Dto
{
    public class ProductResultDto:EntityDto
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string MarketPlace { get; set; }
        public float Rating { get; set; }
        public int ReviewsCount { get; set; }
        public string Image { get; set; }
        public string Specs { get; set; }
        public string Category { get; set; }

    }
}
