using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using WFHSocial.Api.Infrastructure.Data;

namespace WFHSocial.Api.Infrastructure
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public AppDbContextFactory()
        {

        }

        private readonly IConfiguration Configuration;
        public AppDbContextFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public ApplicationDbContext CreateDbContext(string[] args)
        {

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@Directory.GetCurrentDirectory() + @"\appSettings.json")
                .Build();


            ////this works
            //string filePath = @"C:\Users\mike\source\repos\Working From Home Social\WFHSocial.Api.Infrastructure\appSettings.json";


            //IConfiguration configuration = new ConfigurationBuilder()
            //   .SetBasePath(Path.GetDirectoryName(filePath))
            //   .AddJsonFile("appSettings.json")
            //   .Build();


            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
