using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Rating.Product.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Product
{
    public class ProductAppService: CrudAppService<Product, ProductOutputDto, int, PagedAndSortedResultRequestDto, ProductInputDto, ProductInputDto>
    {
        private IRepository<Product> _productRepo;
        public ProductAppService(IRepository<Product> productRepo) : base(productRepo)
        {
            _productRepo = productRepo;
        }

        public override PagedResultDto<ProductOutputDto> GetAll(PagedAndSortedResultRequestDto input)
        {
            return base.GetAll(input);
        }

        public override ProductOutputDto Get(EntityDto<int> input)
        {
            return base.Get(input);
        }

        public ProductOutputDto GetFull(EntityDto<int> input)
        {
            var product = _productRepo.GetAllIncluding(
                x => x.MarketPlace, 
                x=>x.ProductCategory,
                x=> x.Supplier,
                x=> x.Images).Where(x => x.Id == input.Id).FirstOrDefault();

            return ObjectMapper.Map<ProductOutputDto>(product);

        }
        public override ProductOutputDto Create(ProductInputDto input)
        {
            return base.Create(input);
        }
    }
}
