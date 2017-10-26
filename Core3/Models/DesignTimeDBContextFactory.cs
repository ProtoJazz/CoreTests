using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Core3.Models
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BloggingContext>
    {
        public BloggingContext CreateDbContext(string[] args)
        {

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<BloggingContext>();
            string databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            var tokens = databaseUrl.Replace("//", "").Split(':');
            var username = tokens[1];
            var password = tokens[2].Split('@')[0];
            var url = tokens[2].Split('@')[1];
            var port = tokens[3].Split('/')[0];
            var dataBaseName = tokens[3].Split('/')[1];
            string connectionString =
                $"Server={url};Port={port};Database={dataBaseName};User Id={username};Password={password}";
            //services.AddDbContext<BloggingContext>(ServiceLifetime.Scoped);
           // services.AddEntityFrameworkNpgsql().AddDbContext<BloggingContext>(options => options.UseNpgsql(connectionString));


            builder.UseNpgsql(connectionString);

            return new BloggingContext(builder.Options);
        }
    }
}
