using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Rating.Authorization.Roles;
using Rating.Authorization.Users;
using Rating.MultiTenancy;

namespace Rating.EntityFrameworkCore
{
    public class RatingDbContext : AbpZeroDbContext<Tenant, Role, User, RatingDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<Product.Product> Products { get; set; }
        public DbSet<Comment.Comment> Comments { get; set; }
        public DbSet<Image.Image> Images { get; set; }
        public DbSet<Supplier.Supplier> Suppliers { get; set; }
        public DbSet<MarketPlace.MarketPlace> MarketPlaces { get; set; }
        public DbSet<Location.Location> Locations { get; set; }
        public DbSet<ProductCategory.ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductProductFeature.ProductProductFeature> ProductProductFeatures { get; set; }
        public DbSet<ProductFeature.ProductFeature> ProductFeatures { get; set; }
        public DbSet<ProductReview.ProductReview> ProductReviews { get; set; }
        public DbSet<SouqLog.SouqLog> SouqLogs { get; set; }

        public RatingDbContext(DbContextOptions<RatingDbContext> options)
            : base(options)
        {
        }
    }
}
