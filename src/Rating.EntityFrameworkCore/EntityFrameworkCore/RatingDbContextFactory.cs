using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Rating.Configuration;
using Rating.Web;

namespace Rating.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class RatingDbContextFactory : IDesignTimeDbContextFactory<RatingDbContext>
    {
        public RatingDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<RatingDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            RatingDbContextConfigurer.Configure(builder, configuration.GetConnectionString(RatingConsts.ConnectionStringName));

            return new RatingDbContext(builder.Options);
        }
    }
}
