using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Location
{
    public class Location:Entity
    {
        public string Country { get; set; }
        public string LocalCountry { get; set; }
    }
}
