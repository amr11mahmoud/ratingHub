using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Supplier
{
    public class Supplier:Entity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Rating { get; set; }
        public int ReviewsCount { get; set; }
    }
}
