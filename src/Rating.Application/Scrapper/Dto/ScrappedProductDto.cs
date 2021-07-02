using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Scrapper.Dto
{
    public class ScrappedProductDto:EntityDto
    {
        public string Url { get; set; }
    }
}
