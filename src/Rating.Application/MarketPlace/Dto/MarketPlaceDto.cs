using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.MarketPlace.Dto
{
    [AutoMap(typeof(MarketPlace))]
    public class MarketPlaceDto:EntityDto
    {
        public string Name { get; set; }
        public string LocalName { get; set; }
        public string BaseUrl { get; set; }
        public int LocationId { get; set; }

        public int AverageVisitorsCount { get; set; }
    }
}
