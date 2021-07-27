﻿using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.JumiaLog
{
    public class JumiaLog:Entity
    {
        public string Url { get; set; }
        public int CategoryId { get; set; }
        public string Keyword { get; set; }
        public int PageNumber { get; set; }
    }
}
