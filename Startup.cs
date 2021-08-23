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
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

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

            var server = Configuration["DBServer"];
            
            var port = Configuration["DBPort"];
            var user = Configuration["DBUser"];
            var password = Configuration["DBPassword"];
            var database = Configuration["Database"];

            services.AddDbContext<StudentContext>(options =>
                options.UseSqlServer($"Server={server},{port};Initial Catalog={database};User ID ={user};Password={password}",
                sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 })

            ) ;

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "StudentsMicroservice", Version = "v2" });
            });
           
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")

            };
            var rabbitMqConnection = factory.CreateConnection();
            services.AddSingleton(rabbitMqConnection);
            services.AddSingleton<IRabbitMQClient, RabbitMQClient>();


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime)
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



            hostApplicationLifetime.ApplicationStarted.Register(() => { });
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                var rabbitMqClient = app.ApplicationServices.GetRequiredService<IRabbitMQClient>();
                rabbitMqClient.CloseConnection();
            });
        }
    }
}
