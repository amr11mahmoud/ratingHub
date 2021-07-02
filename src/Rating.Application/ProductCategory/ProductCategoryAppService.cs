using Abp.Application.Services;
using Abp.Domain.Repositories;
using Rating.ProductCategory.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.ProductCategory
{
    public class ProductCategoryAppService: CrudAppService<ProductCategory, ProductCategoryDto>
    {
        private IRepository<ProductCategory> _productCategoryRepo;

        public ProductCategoryAppService(IRepository<ProductCategory> productCategoryRepo) : base(productCategoryRepo)
        {
            _productCategoryRepo = productCategoryRepo;
        }
    }
}
