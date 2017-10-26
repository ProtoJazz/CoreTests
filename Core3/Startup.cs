using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core3.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core3.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Core3
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

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

            services.AddEntityFrameworkNpgsql().AddDbContext<BloggingContext>(options => options.UseNpgsql(connectionString));
            services.AddSingleton(Configuration);
            services.AddDbContext<BloggingContext>(ServiceLifetime.Scoped);
            services.AddScoped<IBlogRepository, BlogRepository>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
           
            


            app.UseMvc();
        }
    }
}
