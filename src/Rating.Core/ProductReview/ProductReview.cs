using Abp.Domain.Entities;
using Rating.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.ProductReview
{
    public class ProductReview:Entity
    {
        public string Review { get; set; }
        public float Rate { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }
    }
}
