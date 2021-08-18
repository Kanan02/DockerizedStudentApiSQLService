using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using MyMicroservice.Services;
using EasyNetQ;
namespace MyMicroservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var server = Configuration["DBServer"]??"localhost";
            
            var port = Configuration["DBPort"]??"1433";
            var user = Configuration["DBUser"]?? "SA";
            var password = Configuration["DBPassword"]?? "PaSSw0rd2019";
            var database = Configuration["Database"]??"Students";

            services.AddDbContext<StudentContext>(options =>
                options.UseSqlServer($"Server={server},{port};Initial Catalog={database};User ID ={user};Password={password}",
                sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 })

            ) ;
            //string connection = Configuration.GetConnectionString("DefaultConnection");
            //services.AddDbContext<StudentContext>(options => options.UseSqlServer(connection));
            //services.AddControllersWithViews();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "StudentsMicroservice", Version = "v2" });
            });
            //string rabbitmqConnectionString = "host.docker.internal;username=guest;password=guest;timeout=60";
            //var bus = RabbitHutch.CreateBus(rabbitmqConnectionString);
            //services.AddSingleton(bus);
            //services.AddHostedService<BackgroundServices.UserEventHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "MyMicroservice v2"));
            }

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            PrepDB.PrepPopulation(app);
        }
    }
}
