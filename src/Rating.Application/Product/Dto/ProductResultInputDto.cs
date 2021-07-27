using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Product.Dto
{
    public class ProductResultInputDto
    {
        public string KeyWord { get; set; }
        public int SkipCount { get; set; }
        public int MaxCount { get; set; }
        public int PageNumber { get; set; }
    }
}
