using Abp.Application.Services;
using HtmlAgilityPack;
using Rating.Scrapper.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Abp.Domain.Repositories;
using Rating.Product;
using Rating.Product.Dto;
using Abp.Modules;
using System.ComponentModel;
using Abp.Domain.Uow;
using Abp.UI;
using Abp.Application.Services.Dto;

namespace Rating.Scrapper
{
    public class SouqAppService: CrudAppService<Product.Product, ProductOutputDto>
    {
        private IRepository<Product.Product> _productRepo;
        private IRepository<Supplier.Supplier> _supplierRepo;
        private IRepository<ProductCategory.ProductCategory> _categoryRepo;
        private IRepository<MarketPlace.MarketPlace> _marketPlaceRepo;
        private IRepository<SouqLog.SouqLog> _souqLogRepo;
        private ProductAppService _productAppService;

        public SouqAppService(IRepository<Product.Product> productRepo, 
            IRepository<Supplier.Supplier> supplierRepo,
            IRepository<ProductCategory.ProductCategory> categoryRepo,
            IRepository<MarketPlace.MarketPlace> marketPlaceRepo,
            IRepository<SouqLog.SouqLog> souqLogRepo,
            ProductAppService productAppService):base(productRepo)
        {
            _productRepo = productRepo;
            _supplierRepo = supplierRepo;
            _categoryRepo = categoryRepo;
            _marketPlaceRepo = marketPlaceRepo;
            _souqLogRepo = souqLogRepo;
            _productAppService = productAppService;
        }

        public override PagedResultDto<ProductOutputDto> GetAll(PagedAndSortedResultRequestDto input)
        {
            var marketPlaceId = _marketPlaceRepo.GetAll().Where(x => x.Name == "souq.com").FirstOrDefault().Id;
            var products = 
                _productRepo.GetAllIncluding(x=> x.MarketPlace, x=> x.Supplier, x=> x.ProductCategory)
                .Where(x => x.MarketPlaceId == marketPlaceId);

            return new PagedResultDto<ProductOutputDto>
            {
                TotalCount = products.Count(),
                Items = ObjectMapper.Map<List<ProductOutputDto>>(products)
            };
        }

        public PagedResultDto<ProductOutputDto> GetAllByCategoryId(int categoryId)
        {
            var marketPlaceId = _marketPlaceRepo.GetAll().Where(x => x.Name == "souq.com").FirstOrDefault().Id;
            var products =
                _productRepo.GetAllIncluding(x => x.MarketPlace, x => x.Supplier, x => x.ProductCategory)
                .Where(x => x.MarketPlaceId == marketPlaceId && x.ProductCategId == categoryId);

            return new PagedResultDto<ProductOutputDto>
            {
                TotalCount = products.Count(),
                Items = ObjectMapper.Map<List<ProductOutputDto>>(products)
            };
        }


        public Task<List<Product.Product>> ScrapeSouqEgypt(ScrapSouqDto input)
        {
            string baseUrl = "https://egypt.souq.com/eg-en";
            return GetSouq(input, baseUrl);
        }

        private async Task<List<Product.Product>> GetSouq(ScrapSouqDto input, string baseUrl)
        {
            HtmlWeb web = new HtmlWeb();

            List<string[]> productUrls = new List<string[]>();
            List<Product.Product> products = new List<Product.Product>();

            int id = 1;

            for (int i = 1; i <= input.NumberOfPages; i++)
            {
                for (int j = 1; j < 3; j++)
                {
                    string Url = baseUrl + "/" + input.KeyWord + "/s/?section= " + j + "&page=" + i;

                    HtmlDocument doc = web.Load(Url);
                    GetProductsUrlsSouq(doc, productUrls, id);
                }
            }
            try
            {
                var alreadyScrappedList = _souqLogRepo.GetAll().Where(x => x.Keyword == input.KeyWord).Select(x => x.Url).ToList();
                foreach (var item in productUrls.ToList())
                {
                    if (alreadyScrappedList.Contains(item[0] + "#specs"))
                    {
                        productUrls.Remove(item);
                    }
                }
            }
            catch (Exception x)
            {

                throw x;
            }

            

            foreach (var rec in productUrls)
            {
                try
                {
                    var product = await ScrapProduct(rec, web, products, input);
                    products.Add(product);
                }
                catch (Exception x)
                {

                    throw x;
                }

            }
            return products;
        }

        private async Task<Product.Product> ScrapProduct(string[] rec, HtmlWeb web, List<Product.Product> products, ScrapSouqDto input)
        {
            Supplier.Supplier newSupplier = new Supplier.Supplier();
            Product.Product product = new Product.Product();
            SouqLog.SouqLog souqLog = new SouqLog.SouqLog();

            string en_url = rec[0] + "#specs";
            string ar_url = "https://egypt.souq.com/eg-ar" + en_url.Substring(28);

            HtmlDocument en_doc = web.Load(en_url);
            HtmlDocument ar_doc = web.Load(ar_url);


            string productName = await GetProductNameSouq(en_doc, web);
            string productLocalName = await GetProductNameSouq(ar_doc, web);

            StringBuilder productSpecifications = await GetProductSpecificationsSouq (en_doc, web);
            StringBuilder productLocalSpecifications = await GetProductSpecificationsSouq(ar_doc, web);

            StringBuilder productDescription = await GetProductDescriptionSouq(en_doc, web, 1);
            StringBuilder productLocalDescription = await GetProductDescriptionSouq(ar_doc, web, 2);

            StringBuilder productReviews = await GetProductReviewsSouq(en_doc, web);

            string productImage = rec[1];


            string price = await GetPriceSouq(en_doc, web);

            float productRating = await GetRatingSouq(en_doc, web);
            int productReviewsCount = await GetReviewsCountSouq(en_doc, web);

            int supplierId = 0;
            string[] supplier = await GetSupplierSouq(en_doc, web);

            var suppliers = _supplierRepo.GetAll().Where(x => x.Name == supplier[0]).ToList();

            if (suppliers.Count() == 0)
            {
                newSupplier.Name = supplier[0];
                newSupplier.Url = supplier[1];
                supplierId = _supplierRepo.InsertAndGetId(newSupplier);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            else
            {
                supplierId = suppliers[0].Id;
            }

            product.Name = productName;
            product.LocalName = productLocalName;
            product.Specifications = productSpecifications.ToString();
            product.LocalSpecifications = productLocalSpecifications.ToString();
            product.LocalDescription = productLocalDescription.ToString();
            product.Description = productDescription.ToString();
            product.Reviews = productReviews.ToString();
            product.ProductCategId = input.CategoryId;
            product.MarketPlaceId = input.MarketPlaceId;
            product.SupplierId = supplierId;
            product.Rating = productRating;
            product.ReviewsCount = productReviewsCount;
            product.Url = en_url;
            product.Price = price;
            product.Images = productImage;
            souqLog.Url = en_url;
            souqLog.CategoryId = input.CategoryId;
            souqLog.Keyword = input.KeyWord;


            _productRepo.InsertAndGetId(product);
            _souqLogRepo.InsertAndGetId(souqLog);
            await CurrentUnitOfWork.SaveChangesAsync();

            return product;
        }

        private async Task<string> GetProductNameSouq(HtmlDocument doc, HtmlWeb web)
        {
            var name = "";
            var node = await Task.Run(() => doc.DocumentNode
                .SelectSingleNode("/html/body/div[2]/div/main/div[2]/div/header/div[2]/div[2]/div[2]/div[1]/div/h1"));
            if (node != null)
            {
                name = node.InnerText;
            }
            return name;
        }


        private async Task<float> GetRatingSouq(HtmlDocument doc, HtmlWeb web)
        {
            float rating = 0;

            var productRating =  await Task.Run(() => doc.DocumentNode
                .SelectSingleNode("//*[@id='SOUQ_REVIEWS']/div/div[1]/div/div[1]/div/strong"));

            if (productRating == null)
            {
                productRating = await Task.Run(() => doc.DocumentNode
               .SelectSingleNode("//*[@id='SOUQ_REVIEWS']/div[1]/div[1]/div/div[1]/div/strong"));
            }

            if (productRating != null)
            {
                string ratingStr = productRating.GetAttributeValue("style", string.Empty);
                string ratingValue = Regex.Match(ratingStr, @"\d+").Value;
                rating = float.Parse(productRating.InnerHtml);

            }

            return rating;
        }

        private async Task<int> GetReviewsCountSouq(HtmlDocument doc, HtmlWeb web)
        {
            int count = 0;

            var productReviewsCount = await Task.Run(() => doc.DocumentNode
               .SelectSingleNode("//*[@id='SOUQ_REVIEWS']/div[1]/div[1]/div/div[4]/span"));

            if (productReviewsCount != null)
            {
                string productReviews = productReviewsCount.InnerText;
                string reviewsCount = Regex.Match(productReviews, @"\d+").Value;
                count = Int32.Parse(reviewsCount);
            }

            return count;
        }

        private async Task<StringBuilder> GetProductSpecificationsSouq(HtmlDocument doc, HtmlWeb web)
        {
            var productSpecifications = new StringBuilder();
            productSpecifications.Append("{ ");

            HtmlNode nodes = await Task.Run(()=> doc.DocumentNode.SelectSingleNode("//*[@id='specs-full']/dl"));

            if (nodes == null)
            {
                nodes = await Task.Run(() => doc.DocumentNode.SelectSingleNode("//*[@id='specs-short']/dl"));
            }

            foreach (var node in nodes.ChildNodes)
            {
                if (node.Name == "dt")
                {
                    productSpecifications.Append("'"+ node.InnerText.ToString() + "'" + ":");
                }
                if (node.Name == "dd" && node.FirstChild != null)
                {
                    if (node.FirstChild.Name != "#text")
                    {
                        if (node.InnerHtml == "<i class=\"fi-x\"></i>")
                        {
                            productSpecifications.Append("'" + false + "'" + ",");
                        }
                        else
                        {
                            productSpecifications.Append("'" + true + "'" + ",");
                        }
                    }
                    else
                    {
                        productSpecifications.Append("'" + node.InnerText.ToString() + "'" + ",");
                    }
                }
            }
            
            productSpecifications.Append(" }");

            return productSpecifications;
        }

        private async Task<StringBuilder> GetProductDescriptionSouq(HtmlDocument doc, HtmlWeb web, int en_ar)
        {
            var productDescription = new StringBuilder();
            productDescription.Append("{ ");

            var normalDescNode = await Task.Run(() => doc.DocumentNode.SelectSingleNode("//*[@id='description-full']"));

            if (normalDescNode != null)
            {
                List<HtmlNode> descBullets = new List<HtmlNode>();
                var descNode = normalDescNode.SelectSingleNode("//*[@id='content-body']/div/header/div[2]/div[2]/div[3]/div/div[1]/p[2]");

                for (int i = 1; i < 5; i++)
                {
                    for (int j = 1; j < 6; j++)
                    {
                        HtmlNode node;
                        node = normalDescNode.SelectSingleNode("//*[@id='content-body']/div/header/div[2]/div[2]/div[3]/div/div" + "[" + i + "]" + "/ul" + "[" + j + "]");
                        if (node != null)
                        {
                            descBullets.Add(node);
                        }
                    }
                }

                if (descNode != null)

                {
                    productDescription.Append("'" + "introduction" + "'" + ":");
                    productDescription.Append("'" + descNode.InnerText.ToString() + "'" + ",");
                }

                try
                {
                    foreach (var descBullet in descBullets)
                    {
                        if (descBullet != null)
                        {
                            GetDescriptionBullitSouq(descBullet, productDescription);
                        }
                    }

                }
                catch (Exception x)
                {

                    throw x;
                }

            }
            productDescription.Append(" }");

            return productDescription;
        }

        private void GetDescriptionBullitSouq(HtmlNode descBullit, StringBuilder productDescription)
        {
            foreach (var node in descBullit.ChildNodes)
            {
                var index = node.InnerText.IndexOf(":");
                if (index > 1)
                {
                    var key = node.InnerText.Substring(0, index);
                    var value = node.InnerText.Substring(index + 1);
                    productDescription.Append("'" + key + "'" + ":");
                    productDescription.Append("'" + value + "'" + ",");
                }
            }
        }

        private async Task<StringBuilder> GetProductReviewsSouq(HtmlDocument doc, HtmlWeb web)
        {
            var productReviews = new StringBuilder();
            productReviews.Append("[ ");

            var reviewsRoot = await Task.Run(()=>doc.DocumentNode.SelectSingleNode("//*[@id='reviews-list-id']"));

            if (reviewsRoot != null)
            {
                foreach (var review in reviewsRoot.SelectNodes("li"))
                {
                    productReviews.Append("{ ");

                    var user = review.SelectNodes("header/div[contains(@class, 'by-date')]/span/strong").ToList()[0].InnerText;
                    productReviews.Append("'User" + "'" + ":");
                    productReviews.Append("'" + user.ToString() + "'" + ",");

                    var rating = review.SelectSingleNode("header/div[contains(@class, 'space')]/span/i/i").GetAttributeValue("style", string.Empty);
                    var ratingValue = Regex.Match(rating.ToString(), @"\d+").Value;

                    productReviews.Append("'Rating" + "'" + ":");
                    productReviews.Append("'" + float.Parse(ratingValue) / 20 + "'" + ",");

                    var comment = review.SelectSingleNode("article/p").InnerText;
                    productReviews.Append("'Comment" + "'" + ":");
                    productReviews.Append("'" + comment.ToString() + "'" + ",");

                    productReviews.Append(" },");

                }
            }

            productReviews.Append(" ]");

            return productReviews;
        }
        private async Task<string[]> GetSupplierSouq(HtmlDocument doc, HtmlWeb web)
        {
            string[] supplier = new string[2];

            try
            {
                HtmlNode nodes;
                for (int i = 1; i < 10; i++)
                {
                     nodes = await Task.Run(() => doc.DocumentNode
                    .SelectSingleNode("//*[@id='content-body']/div/header/div[2]/div[2]/div"+"["+i+"]"+"/dl/dd/span/a"));

                    if (nodes != null)
                    {
                        string productSupplier = nodes.SelectSingleNode("b").InnerText;
                        string productSupplierUrl = nodes.GetAttributeValue("href", string.Empty);
                        supplier[0] = productSupplier;
                        supplier[1] = productSupplierUrl;
                        break;
                    }
                }

            }
            catch (Exception x )
            {

                throw x;
            }            
            
            return supplier;
        }

        private async Task<string> GetPriceSouq(HtmlDocument doc, HtmlWeb web)
        {

            string price = "0";

            var productPrice = await Task.Run(() => doc.DocumentNode
               .SelectSingleNode("//*[@id='content-body']/div/header/div[2]/div[2]/div[3]/div/section/div/div/div[1]/h3"));

            if (productPrice != null)
            {
                string productPriceStr = productPrice.InnerText;
                price = Regex.Match(productPriceStr, @"(?:\+|\-|\$)?\d{1,}(?:\,?\d{3})*(?:\.\d+)?%?").Value;
            }

            return price;
        }
        private void GetProductsUrlsSouq(HtmlDocument doc, List<string[]> Urls, int id)
        {
            var allProductsNodes = doc.DocumentNode.SelectSingleNode(" / html/body/div[2]/div/main/div[2]/div[2]/div[7]/div/div")
                .SelectNodes("//div[contains(@class, 'single-item')]");

            foreach (var node in allProductsNodes)
            {
                var url = node.SelectSingleNode("div[2]/a").GetAttributeValue("href", string.Empty);
                var image = node.SelectSingleNode("div/a/img").GetAttributeValue("data-src", string.Empty);
                var imgSrcXl = image.Replace("item_L_", "item_XL_");

                string[] urlAndImage = { url, imgSrcXl };
                Urls.Add(urlAndImage);
                id++;

            }
        }
    }
}
