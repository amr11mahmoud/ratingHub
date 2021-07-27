using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Rating.Product.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTextCat;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Rating.Product
{
    public class ProductAppService: CrudAppService<Product, ProductOutputDto, int, PagedAndSortedResultRequestDto, ProductInputDto, ProductInputDto>
    {
        private IRepository<Product> _productRepo;
        private ProductManager _productManager;

        public ProductAppService(IRepository<Product> productRepo, ProductManager productManager) : base(productRepo)
        {
            _productRepo = productRepo;
            _productManager = productManager;
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

            var product = _productRepo.GetAll()
                .Include(product => product.OnSiteReviews)
                .Include(product => product.MarketPlace)
                .Include(product => product.Supplier)
                .Include(product => product.ProductCategory)
                
                .Where(x => x.Id == input.Id).FirstOrDefault();

            return ObjectMapper.Map<ProductOutputDto>(product);

        }
        
        public override ProductOutputDto Create(ProductInputDto input)
        {
            return base.Create(input);
        }

        public PagedResultDto<string> SearchRecommendation(ProductResultInputDto input)
        {
            bool arKeyword = Regex.Match(input.KeyWord, @"[\u0621-\u064A]+").Success;

            if (arKeyword)
            {
                var searchResult = _productRepo.GetAll().Where(x => x.LocalName.Contains(input.KeyWord)).Select(x => x.LocalName)
                    .Take(10).ToList();

                return new PagedResultDto<string>
                {
                    TotalCount = searchResult.Count(),
                    Items = ObjectMapper.Map<List<string>>(searchResult)
                };
            }
            else
            {
                var searchResult = _productRepo.GetAll().Where(x => x.Name.Contains(input.KeyWord)).Select(x => x.Name)
                    .Take(10).ToList();

                return new PagedResultDto<string>
                {
                    TotalCount = searchResult.Count(),
                    Items = ObjectMapper.Map<List<string>>(searchResult)
                };
            }
        }

        public PagedResultDto<ProductResultDto> GetProductsByKeyword(ProductResultInputDto input)
        {
            List<ProductResultDto> productsList = new List<ProductResultDto>();
            bool arKeyword = Regex.Match(input.KeyWord, @"[\u0621-\u064A]+").Success;

            if (arKeyword)
            {
                var products = 
                    _productRepo.GetAll().Where(x => x.LocalName.Contains(input.KeyWord))
                    .Select(x => new {
                        x.Id ,
                        x.LocalName, 
                        x.Price, 
                        x.MarketPlace, 
                        x.Rating, 
                        x.ReviewsCount, 
                        x.Images,
                        x.LocalSpecifications,
                        x.ProductCategory}).Skip(input.SkipCount).Take(input.MaxCount).ToList();
                

                foreach (var product in products)
                {
                    string marketPlace = product.MarketPlace.Name;
                    ProductResultDto newProduct = new ProductResultDto();
                    newProduct.Name = product.LocalName;
                    newProduct.MarketPlace = marketPlace;
                    newProduct.Price = product.Price;
                    newProduct.Rating = product.Rating;
                    newProduct.ReviewsCount = product.ReviewsCount;
                    newProduct.Image = product.Images;
                    newProduct.Id = product.Id;
                    newProduct.Category = product.ProductCategory.Name;
                    newProduct.Specs = product.LocalSpecifications;
                    productsList.Add(newProduct);
                }

                return new PagedResultDto<ProductResultDto>
                {
                    TotalCount = productsList.Count(),
                    Items = ObjectMapper.Map<List<ProductResultDto>>(productsList)
                };
            }
            else
            {
                var products =
                    _productRepo.GetAll().Where(x => x.Name.Contains(input.KeyWord))
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Price,
                        x.MarketPlace,
                        x.Rating,
                        x.ReviewsCount,
                        x.Images,
                        x.Specifications,
                        x.ProductCategory
                    }).Skip(input.SkipCount).Take(input.MaxCount).ToList();

                foreach (var product in products)
                {
                    ProductResultDto newProduct = new ProductResultDto();
                    string marketPlace = product.MarketPlace.Name;
                    newProduct.Name = product.Name;
                    newProduct.MarketPlace = marketPlace;
                    newProduct.Price = product.Price;
                    newProduct.Rating = product.Rating;
                    newProduct.ReviewsCount = product.ReviewsCount;
                    newProduct.Image = product.Images;
                    newProduct.Id = product.Id;
                    newProduct.Category = product.ProductCategory.Name;
                    newProduct.Specs = product.Specifications;
                    productsList.Add(newProduct);
                }

                return new PagedResultDto<ProductResultDto>
                {
                    TotalCount = productsList.Count(),
                    Items = ObjectMapper.Map<List<ProductResultDto>>(productsList)
                };
            }

        }
         
        public PagedResultDto<ProductResultDto> GetProductsByCategory(ProductResultInputDto input)
        {
            List<ProductResultDto> productsList = new List<ProductResultDto>();

            var products =
                    _productRepo.GetAll().Where(x => x.ProductCategId == Int32.Parse(input.KeyWord))
                    .Select(x => new {
                        x.Id,
                        x.Name,
                        x.Price,
                        x.MarketPlace,
                        x.Rating,
                        x.ReviewsCount,
                        x.Images,
                        x.Specifications,
                        x.ProductCategory
                    }).Skip(input.SkipCount).Take(input.MaxCount).ToList();

            foreach (var product in products)
            {
                string marketPlace = product.MarketPlace.Name;
                ProductResultDto newProduct = new ProductResultDto();
                newProduct.Name = product.Name;
                newProduct.MarketPlace = marketPlace;
                newProduct.Price = product.Price;
                newProduct.Rating = product.Rating;
                newProduct.ReviewsCount = product.ReviewsCount;
                newProduct.Image = product.Images;
                newProduct.Id = product.Id;
                newProduct.Category = product.ProductCategory.Name;
                newProduct.Specs = product.Specifications;
                productsList.Add(newProduct);
            }

            return new PagedResultDto<ProductResultDto>
            {
                TotalCount = productsList.Count(),
                Items = ObjectMapper.Map<List<ProductResultDto>>(productsList)
            };
        }
    }
}
