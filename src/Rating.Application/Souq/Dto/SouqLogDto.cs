using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Souq.Dto
{
    [AutoMap(typeof(SouqLog.SouqLog))]
    public class SouqLogDto:EntityDto
    {
        public string Url { get; set; }
        public int CategoryId { get; set; }
        public string Keyword { get; set; }
        public int PageNumber { get; set; }
    }
}
