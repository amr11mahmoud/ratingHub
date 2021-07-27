using Abp.Application.Services;
using Abp.Domain.Repositories;
using Rating.ProductReview.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.ProductReview
{
    public class ProductReviewAppService: CrudAppService<ProductReview, ProductReviewDto>
    {
        private IRepository<ProductReview> _productReviewRepo;

        public ProductReviewAppService(IRepository<ProductReview> productReviewRepo):base(productReviewRepo)
        {
            _productReviewRepo = productReviewRepo;
        }
    }
}
