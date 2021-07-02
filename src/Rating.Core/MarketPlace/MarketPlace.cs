using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.MarketPlace
{
    public class MarketPlace:Entity
    {
        public string Name { get; set; }
        public string LocalName { get; set; }
        public string BaseUrl { get; set; }
        public string Logo { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        public int AverageVisitorsCount { get; set; }

        public Location.Location Location { get; set; }

    }
}
