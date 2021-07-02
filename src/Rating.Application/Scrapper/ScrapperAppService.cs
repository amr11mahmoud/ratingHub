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

namespace Rating.Scrapper
{
    public class ScrapperAppService:IApplicationService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private IRepository<Product.Product> _productRepo;
        private IRepository<Supplier.Supplier> _supplierRepo;
        private ProductAppService _productAppService;
        private static readonly Queue<Task> WaitingTasks = new Queue<Task>();
        private static readonly Dictionary<int, Task> RunningTasks = new Dictionary<int, Task>();
        public static int MaxRunningTasks = 100; // vary this to dynamically throttle launching new tasks 


        public ScrapperAppService(IRepository<Product.Product> productRepo
            , IRepository<Supplier.Supplier> supplierRepo,
            IUnitOfWorkManager unitOfWorkManager,
            ProductAppService productAppService)
        {
            _productRepo = productRepo;
            _supplierRepo = supplierRepo;
            _productAppService = productAppService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public List<int> GetSouqEgypt(ScrapSouqDto input)
        {
            HtmlWeb web = new HtmlWeb();

            List<string> productUrls = new List<string>();
            List<int> products = new List<int>();

            int id = 1;

            for (int i = 1; i <= input.NumberOfPages; i++)
            {
                for (int j = 1; j < 3; j++)
                {
                    string baseUrl = "https://egypt.souq.com/eg-en" + "/" + input.KeyWord + "/s/?section= " + j + "&page=" + i;

                    HtmlDocument doc = web.Load(baseUrl);
                    GetProductsUrlsSouq(doc, productUrls, id);
                }
            }

            foreach (var rec in productUrls)
            {
                ScrapProduct(rec, web, products, input);
            }

            return products;
        }

        private void ScrapProduct(string rec, HtmlWeb web, List<int> products, ScrapSouqDto input)
        {
            Supplier.Supplier newSupplier = new Supplier.Supplier();
            Product.Product product = new Product.Product();

            string en_url = rec + "#specs";
            string ar_url = "https://egypt.souq.com/eg-ar" + en_url.Substring(28);

            HtmlDocument en_doc = web.Load(en_url);
            HtmlDocument ar_doc = web.Load(ar_url);


            string productName = GetProductNameSouq(en_doc, web);
            string productLocalName = GetProductNameSouq(ar_doc, web);

            StringBuilder productSpecifications = GetProductSpecificationsSouq(en_doc, web);
            StringBuilder productLocalSpecifications = GetProductSpecificationsSouq(ar_doc, web);

            StringBuilder productDescription = GetProductDescriptionSouq(en_doc, web, 1);
            StringBuilder productLocalDescription = GetProductDescriptionSouq(ar_doc, web, 2);

            StringBuilder productReviews = GetProductReviewsSouq(en_doc, web);

            float productRating = GetRatingSouq(en_doc, web);
            int productReviewsCount = GetReviewsCountSouq(en_doc, web);

            string[] supplier = GetSupplierSouq(en_doc, web);
            int supplierId = 0;
            var suppliers = _supplierRepo.GetAll().Where(x => x.Name == supplier[0]).ToList();

            if (suppliers.Count() == 0)
            {
                newSupplier.Name = supplier[0];
                newSupplier.Url = supplier[1];
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    supplierId = _supplierRepo.InsertAndGetId(newSupplier);

                    unitOfWork.Complete();
                }
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

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                int product_id = _productRepo.InsertAndGetId(product);
                unitOfWork.Complete();
                products.Add(product_id);
            }
        }

        private string GetProductNameSouq(HtmlDocument doc, HtmlWeb web)
        {
            string productName = 
                doc.DocumentNode
                .SelectSingleNode("/html/body/div[2]/div/main/div[2]/div/header/div[2]/div[2]/div[2]/div[1]/div/h1")
                .InnerText;
            return productName;
        }

        private float GetRatingSouq(HtmlDocument doc, HtmlWeb web)
        {
            float rating = 0;

            var productRating =
                doc.DocumentNode
                .SelectSingleNode("//*[@id='content-body']/div/header/div[2]/div[2]/div[2]/div[1]/div/span[2]/a[1]/i[1]/i");
            if (productRating != null)
            {
                string ratingStr = productRating.GetAttributeValue("style", string.Empty);
                string ratingValue = Regex.Match(ratingStr, @"\d+").Value;
                rating = float.Parse(ratingValue) / 20;

            }
            return rating;
        }

        private int GetReviewsCountSouq(HtmlDocument doc, HtmlWeb web)
        {
            int count = 0;
            var productReviewsCount =
                doc.DocumentNode
                .SelectSingleNode("//*[@id='content-body']/div/header/div[2]/div[2]/div[2]/div[1]/div/span[2]/a[2]/span");

            if (productReviewsCount != null)
            {
                string productReviews = productReviewsCount.InnerText;
                string reviewsCount = Regex.Match(productReviews, @"\d+").Value;
                count = Int32.Parse(reviewsCount);
            }
            

            return count;
        }

        private StringBuilder GetProductSpecificationsSouq(HtmlDocument doc, HtmlWeb web)
        {
            var productSpecifications = new StringBuilder();
            productSpecifications.Append("{ ");

            var nodes = doc.DocumentNode.SelectSingleNode("//*[@id='specs-full']/dl");

            foreach (var node in nodes.ChildNodes)
            {
                if (node.Name == "dt")
                {
                    productSpecifications.Append("'"+ node.InnerText.ToString() + "'" + ":");
                }
                if (node.Name == "dd")
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

        private StringBuilder GetProductDescriptionSouq(HtmlDocument doc, HtmlWeb web, int en_ar)
        {
            var productDescription = new StringBuilder();
            productDescription.Append("{ ");

            var nodes = doc.DocumentNode.SelectSingleNode("//*[@id='description-full']");
            if (nodes != null)
            {
                foreach (var node in nodes.ChildNodes)
                {
                    if (node.Name == "p")
                    {
                        productDescription.Append("'" + "introduction" + "'" + ":");
                        productDescription.Append("'" + node.InnerText.ToString() + "'" + ",");
                    }
                    if (node.Name == "ul")
                    {
                        foreach (var item in node.ChildNodes)
                        {
                            if (en_ar == 2)
                            {
                                string arabText = System.Net.WebUtility.HtmlDecode(item.InnerText.ToString());
                                int key_index = arabText.IndexOf(":");

                                productDescription.Append("'");
                                productDescription.Append(arabText.Substring(0, key_index - 1));
                                productDescription.Append("':");
                                productDescription.Append("'");
                                productDescription.Append(arabText.Substring(key_index + 1));
                                productDescription.Append("',");
                            }
                            else
                            {
                                productDescription.Append("'" + item.InnerText.ToString() + "'" + ",");
                            }

                        }
                    }
                }
            }

            productDescription.Append(" }");

            return productDescription;
        }
        
        private StringBuilder GetProductReviewsSouq(HtmlDocument doc, HtmlWeb web)
        {
            var productReviews = new StringBuilder();
            productReviews.Append("[ ");

            var reviewsRoot = doc.DocumentNode.SelectSingleNode("//*[@id='reviews-list-id']");

            if (reviewsRoot != null)
            {
                foreach (var review in reviewsRoot.SelectNodes("li"))
                {
                    productReviews.Append("{ ");

                    var user = review.SelectNodes("header/div[contains(@class, 'by-date')]/span/strong").ToList()[0].InnerText;
                    productReviews.Append("'User" + "'" + ":");
                    productReviews.Append("'" + user.ToString() + "'" + ",");

                    var rating = review.SelectSingleNode("header/div[contains(@class, 'space')]/span/i/i").GetAttributeValue("style", string.Empty);
                    productReviews.Append("'Rating" + "'" + ":");
                    productReviews.Append("'" + rating.ToString() + "'" + ",");

                    var comment = review.SelectSingleNode("article/p").InnerText;
                    productReviews.Append("'Comment" + "'" + ":");
                    productReviews.Append("'" + comment.ToString() + "'" + ",");

                    productReviews.Append(" },");

                }
            }

            productReviews.Append(" ]");

            return productReviews;
        }
        private string[] GetSupplierSouq(HtmlDocument doc, HtmlWeb web)
        {
            var nodes= doc.DocumentNode
                .SelectSingleNode("//*[@id='content-body']/div/header/div[2]/div[2]/div[4]/dl/dd/span/a");

            string productSupplier = nodes.SelectSingleNode("b").InnerText;
            string productSupplierUrl = nodes.GetAttributeValue("href", string.Empty);
            string[] supplier = { productSupplier, productSupplierUrl };

            return supplier;
        }

        private void GetProductsUrlsSouq(HtmlDocument doc, List<string> Urls, int id)
        {
            var allProductsNodes = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/main/div[2]/div[2]/div[7]/div/div")
                .SelectNodes("//div[contains(@class, 'single-item')]");

            foreach (var node in allProductsNodes)
            {
                if (id < 10)
                {
                    var url = node.SelectSingleNode("div[2]/a").GetAttributeValue("href", string.Empty);
                    //record.Url = url;
                    //record.Id = id;
                    Urls.Add(url);
                    id++;
                }
                
            }

            //foreach (var node in doc.DocumentNode.SelectNodes("//*[@id='content-body']"))
            //{
            //    foreach (var node2 in node.SelectNodes("//div[contains(@class, 'tpl-results')]"))
            //    {
            //        foreach (var node3 in node2.SelectNodes("//div[contains(@class, 'list-view')]"))
            //        {
            //            foreach (var node4 in node3.SelectNodes("//div[contains(@class, 'tpl-append-results')]"))
            //            {
            //                foreach (var node5 in node4.SelectNodes("//div[contains(@class, 'column')]"))
            //                {
            //                    foreach (var node6 in node5.SelectNodes("//div[contains(@class, 'item-content')]"))
            //                    {
            //                        foreach (var node7 in node6.ChildNodes.ToList())
            //                        {
            //                            if (node7.Name == "a")
            //                            {
            //                                ScrappedProductDto record = new ScrappedProductDto();
            //                                record.Url = node7.GetAttributeValue("href", string.Empty);
            //                                //record.Name = String.Join("", node7.GetAttributeValue("title", string.Empty).Split('\n', '\t'));
            //                                record.Id = id;
            //                                lstRecords.Add(record);
            //                                id++;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}
