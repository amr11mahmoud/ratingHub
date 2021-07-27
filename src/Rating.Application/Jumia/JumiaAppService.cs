using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using HtmlAgilityPack;
using Rating.Product;
using Rating.Product.Dto;
using Rating.Scrapper.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Jumia
{
    public class JumiaAppService: CrudAppService<Product.Product, ProductOutputDto>
    {
        private IRepository<Product.Product> _productRepo;
        private IRepository<Supplier.Supplier> _supplierRepo;
        private IRepository<ProductCategory.ProductCategory> _categoryRepo;
        private IRepository<MarketPlace.MarketPlace> _marketPlaceRepo;
        private IRepository<JumiaLog.JumiaLog> _jumiaLogRepo;
        private ProductAppService _productAppService;


        public JumiaAppService(IRepository<Product.Product> productRepo,
            IRepository<Supplier.Supplier> supplierRepo,
            IRepository<ProductCategory.ProductCategory> categoryRepo,
            IRepository<MarketPlace.MarketPlace> marketPlaceRepo,
            IRepository<JumiaLog.JumiaLog> jumiaLogRepo,
            ProductAppService productAppService) : base(productRepo)
        {
            _productRepo = productRepo;
            _supplierRepo = supplierRepo;
            _categoryRepo = categoryRepo;
            _marketPlaceRepo = marketPlaceRepo;
            _jumiaLogRepo = jumiaLogRepo;
            _productAppService = productAppService;
        }

        public override PagedResultDto<ProductOutputDto> GetAll(PagedAndSortedResultRequestDto input)
        {
            var marketPlaceId = _marketPlaceRepo.GetAll().Where(x => x.Name == "jumia").FirstOrDefault().Id;
            var products =
                _productRepo.GetAllIncluding(x => x.MarketPlace, x => x.Supplier, x => x.ProductCategory)
                .Where(x => x.MarketPlaceId == marketPlaceId);

            return new PagedResultDto<ProductOutputDto>
            {
                TotalCount = products.Count(),
                Items = ObjectMapper.Map<List<ProductOutputDto>>(products)
            };
        }

        public PagedResultDto<ProductOutputDto> GetAllByCategoryId(int categoryId)
        {
            var marketPlaceId = _marketPlaceRepo.GetAll().Where(x => x.Name == "jumia").FirstOrDefault().Id;
            var products =
                _productRepo.GetAllIncluding(x => x.MarketPlace, x => x.Supplier, x => x.ProductCategory)
                .Where(x => x.MarketPlaceId == marketPlaceId && x.ProductCategId == categoryId);

            return new PagedResultDto<ProductOutputDto>
            {
                TotalCount = products.Count(),
                Items = ObjectMapper.Map<List<ProductOutputDto>>(products)
            };
        }

        public Task<List<Product.Product>> ScrapeJumiaEgypt(ScrapSouqDto input)
        {
            try
            {
                string baseUrl = "https://www.jumia.com.eg/";
                return GetJumia(input, baseUrl);
            }
            catch (Exception x)
            {

                throw x;
            }

        }

        private void GetProductsUrlsJumia(HtmlDocument doc, List<string[]> Urls, int id)
        {
            var allProductsNodes = doc.DocumentNode.SelectSingleNode("//*[@id='jm']/main/div[2]/div[3]/section/div[1]").ChildNodes;

            foreach (var node in allProductsNodes)
            {
                var urlNode = node.SelectSingleNode("a");
                string url = "";

                if (urlNode != null)
                {
                    url = "https://www.jumia.com.eg/" + urlNode.GetAttributeValue("href", string.Empty);
                }
                //else
                //{
                //    url = node.SelectSingleNode("div/div[2]/ul/li[1]/h6/a").GetAttributeValue("href", string.Empty);
                //}

                var imageNode = node.SelectSingleNode("a/div/img");
                string image = "";
                var imgSrcXl = "";

                if (imageNode != null)
                {

                    image = imageNode.GetAttributeValue("data-src", string.Empty);
                    imgSrcXl = image.Replace("300x300", "600x600");
                }
                //else
                //{
                //    image = node.SelectSingleNode("div/div/div/a").GetAttributeValue("data-img", string.Empty);
                //    imgSrcXl = image.Replace("300x300", "600x600");
                //}

                if (url != "https://www.jumia.com.eg/")
                {
                    string[] urlAndImage = { url, imgSrcXl };
                    Urls.Add(urlAndImage);
                    id++;
                }
            }
        }

        private async Task<List<Product.Product>> GetJumia(ScrapSouqDto input, string baseUrl)
        {
            HtmlWeb web = new HtmlWeb();
            //WebClient webClient = new WebClient();

            List<string[]> productUrls = new List<string[]>();
            List<Product.Product> products = new List<Product.Product>();

            int id = 1;

            for (int i = 1; i <= input.NumberOfPages; i++)
            {
                string Url = baseUrl + "/catalog/?q=" + input.KeyWord + "&page=" + i;

                HtmlDocument doc = web.Load(Url);
                //IPage doc2 = webClient.GetPage(Url);
                GetProductsUrlsJumia(doc, productUrls, id);
            }
            try
            {
                var alreadyScrappedList = _jumiaLogRepo.GetAll().Where(x => x.Keyword == input.KeyWord).Select(x => x.Url).ToList();
                foreach (var item in productUrls.ToList())
                {
                    if (alreadyScrappedList.Contains(item[0]))
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
            JumiaLog.JumiaLog jumiaLog = new JumiaLog.JumiaLog();

            string en_url = rec[0];
            string ar_url = "https://www.jumia.com.eg/ar" + en_url.Substring(25);

            HtmlDocument en_doc = web.Load(en_url);
            HtmlDocument ar_doc = web.Load(ar_url);

            string productName = await GetProductNameJumia(en_doc, web);
            string productLocalName = await GetProductNameJumia(ar_doc, web);


            StringBuilder productSpecifications = await GetProductSpecificationsJumia(en_doc, web);
            StringBuilder productLocalSpecifications = await GetProductSpecificationsJumia(ar_doc, web);

            //StringBuilder productDescription = await GetProductDescriptionSouq(en_doc, web, 1);
            //StringBuilder productLocalDescription = await GetProductDescriptionSouq(ar_doc, web, 2);

            //StringBuilder productReviews = await GetProductReviewsSouq(en_doc, web);

            //string productImage = rec[1];


            //string price = await GetPriceSouq(en_doc, web);

            //float productRating = await GetRatingSouq(en_doc, web);
            //int productReviewsCount = await GetReviewsCountSouq(en_doc, web);

            //int supplierId = 0;
            //string[] supplier = await GetSupplierSouq(en_doc, web);

            //var suppliers = _supplierRepo.GetAll().Where(x => x.Name == supplier[0]).ToList();

            //if (suppliers.Count() == 0)
            //{
            //    newSupplier.Name = supplier[0];
            //    newSupplier.Url = supplier[1];
            //    supplierId = _supplierRepo.InsertAndGetId(newSupplier);
            //    await CurrentUnitOfWork.SaveChangesAsync();
            //}
            //else
            //{
            //    supplierId = suppliers[0].Id;
            //}

            //product.Name = productName;
            //product.LocalName = productLocalName;
            //product.Specifications = productSpecifications.ToString();
            //product.LocalSpecifications = productLocalSpecifications.ToString();
            //product.LocalDescription = productLocalDescription.ToString();
            //product.Description = productDescription.ToString();
            //product.Reviews = productReviews.ToString();
            //product.ProductCategId = input.CategoryId;
            //product.MarketPlaceId = input.MarketPlaceId;
            //product.SupplierId = supplierId;
            //product.Rating = productRating;
            //product.ReviewsCount = productReviewsCount;
            //product.Url = en_url;
            //product.Price = price;
            //product.Images = productImage;
            //souqLog.Url = en_url;
            //souqLog.CategoryId = input.CategoryId;
            //souqLog.Keyword = input.KeyWord;


            //_productRepo.InsertAndGetId(product);
            //_souqLogRepo.InsertAndGetId(souqLog);
            //await CurrentUnitOfWork.SaveChangesAsync();

            return product;
        }

        private async Task<string> GetProductNameJumia(HtmlDocument doc, HtmlWeb web)
        {
            var name = "";
            var node = await Task.Run(() => doc.DocumentNode
                .SelectSingleNode("//*[@id='jm']/main/div[1]/section/div/div[2]/div[1]/div/h1"));
            if (node != null)
            {
                name = node.InnerText;
            }
            return name;
        }

        private async Task<StringBuilder> GetProductSpecificationsJumia(HtmlDocument doc, HtmlWeb web)
        {
            var productSpecifications = new StringBuilder();
            productSpecifications.Append("{ ");

            HtmlNode nodes = await Task.Run(() => doc.DocumentNode.SelectSingleNode("//*[@id='jm']/main/div[2]/div[2]/div[1]/div[2]"));

            if (nodes != null)
            {
                foreach (var node in nodes.ChildNodes)
                {

                    if (node.Name == "ul" && node.FirstChild != null)
                    {
                        foreach (var item in node.ChildNodes)
                        {
                            if (item.InnerText.ToString().Length > 0)
                            {
                                productSpecifications.Append("'" + item.InnerText.ToString() + "'" + ",");
                            }
                            else
                            {
                                productSpecifications.Append("'" + "Empty" + "'" + ",");
                            }
                        }
                    }
                }
            }

            productSpecifications.Append(" }");

            return productSpecifications;
        }
    }
}
