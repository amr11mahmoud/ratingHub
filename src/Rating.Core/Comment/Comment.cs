using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Comment
{
    public class Comment:Entity
    {
        public int Body { get; set; }
        public string Date { get; set; }
        public string Commenter { get; set; }
        public int? UserId { get; set; }
        public int ProductId { get; set; }

    }
}
