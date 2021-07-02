using Abp.Application.Services;
using Abp.Domain.Repositories;
using Rating.MarketPlace.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.MarketPlace
{
    public class MarketPlaceAppService : CrudAppService<MarketPlace, MarketPlaceDto> 
    {
        private IRepository<MarketPlace> _marketPlaceRepo;

        public MarketPlaceAppService(IRepository<MarketPlace> marketPlaceRepo):base(marketPlaceRepo)
        {
            _marketPlaceRepo = marketPlaceRepo;
        }
    }
    
}
