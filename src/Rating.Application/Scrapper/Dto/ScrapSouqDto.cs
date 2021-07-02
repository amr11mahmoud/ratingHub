using Abp.Application.Services.Dto;
using Rating.ProductCategory.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Scrapper.Dto
{
    public class ScrapSouqDto:EntityDto
    {
        public int NumberOfPages { get; set; }
        public string KeyWord { get; set; }
        public int CategoryId { get; set; }
        public int MarketPlaceId { get; set; }

    }
}
