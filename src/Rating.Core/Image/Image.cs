using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Image
{
    public class Image:Entity
    {
        public string Url { get; set; }
        public int ProductId { get; set; }
    }
}
