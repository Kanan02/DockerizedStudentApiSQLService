using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyMicroservice.Services;

namespace MyMicroservice.Models
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder application)
        {
            using (var serviceScope = application.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<StudentContext>());
            }
        }
        public static void SeedData(StudentContext context)
        {
            Console.WriteLine("Appling Migrations..");
            context.Database.Migrate();

            if (!context.Students.Any())
            {
                Console.WriteLine("Adding data - seeding");
                context.Students.AddRange(
                    new Student()
                    {
                        Email = "a@gmail.com",
                        Name = "A",
                        Surname = "AA",
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                       
                    },
                    new Student()
                    {
                        Email = "b@gmail.com",
                        Name = "B",
                        Surname = "BB",
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    
                    }
                    ) ;
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Already have data - not seeding");
            }
        }
    }
}
