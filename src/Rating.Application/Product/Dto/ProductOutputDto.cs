using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating.Product.Dto
{
    [AutoMap(typeof(Product))]
    public class ProductOutputDto: EntityDto
    {
        public string Name { get; set; }
        public string LocalName { get; set; }
        public string Description { get; set; }
        public string LocalDescription { get; set; }
        public string Specifications { get; set; }
        public string LocalSpecifications { get; set; }
        public string Reviews { get; set; }
        public string Comments { get; set; }
        public string LocalReviews { get; set; }
        [ForeignKey("ProductCategory")]
        public int ProductCategId { get; set; }
        [ForeignKey("MarketPlace")]
        public int MarketPlaceId { get; set; }
        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public string Url { get; set; }
        public float Rating { get; set; }
        public int ReviewsCount { get; set; }
        public MarketPlace.MarketPlace MarketPlace { get; set; }
        public ICollection<Image.Image> Images { get; set; }
        public Supplier.Supplier Supplier { get; set; }
        public ProductCategory.ProductCategory ProductCategory { get; set; }
    }
}
