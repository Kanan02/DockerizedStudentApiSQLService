using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyMicroservice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyMicroservice.Services
{
    public class StudentContext:DbContext
    {
        public StudentContext()
        {

        }
        public StudentContext(DbContextOptions<StudentContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }

        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            return Students;
        }

        public object GetById(long id)
        {
            return Students.FirstOrDefault(t => t.Id == id);
        }

        public bool Create([FromBody] Student item)
        {
            if (item == null)
            {
                return false;
            }
            item.InsertedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;

            Students.Add(item);
            SaveChanges();

            return true;
        }

        public bool Update(long id, [FromBody] Student item)
        {
            var student = Students.FirstOrDefault(t => t.Id == id);
            if (student == null)
            {
                return false;
            }
            student.Email = item.Email;
            student.Id = item.Id;
            student.InsertedAt = item.InsertedAt;
            student.Name = item.Name;
            student.Surname = item.Surname;
            student.UpdatedAt = DateTime.Now;
            Students.Update(student);
            SaveChanges();
            return true;
        }
        public bool Delete(long id)
        {
            var student = Students.FirstOrDefault(t => t.Id == id);
            if (student == null)
            {
                return false;
            }
            Students.Remove(student);
            SaveChanges();
            return true;
        }
    }
}
