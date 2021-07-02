using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Rating.EntityFrameworkCore
{
    public static class RatingDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<RatingDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<RatingDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
