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
    public class StudentContext:DbContext,IStudentService
    {
        public StudentContext()
        {

        }
        public StudentContext(DbContextOptions<StudentContext> options) : base(options)
        {

        }
        public  DbSet<Student> Students { get; set; }

        public IEnumerable<Student> GetStudents()
        {
            return Students;
        }

        public Student GetById(long id)
        {
            return Students.FirstOrDefault(t => t.Id == id);
        }

        public Student Create(Student item)
        {
            item.InsertedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;

            Students.Add(item);
            SaveChanges();

            return item;
        }

        public Student Update(long id, Student item)
        {
            var student = Students.FirstOrDefault(t => t.Id == id);
          
            student.Email = item.Email;
            student.Id = item.Id;
            student.InsertedAt = item.InsertedAt;
            student.Name = item.Name;
            student.Surname = item.Surname;
            student.UpdatedAt = DateTime.Now;
            Students.Update(student);
            SaveChanges();
            return student;
        }
        public void Delete(long id)
        {
            var student = Students.FirstOrDefault(t => t.Id == id);
            Students.Remove(student);
            SaveChanges();
        }
    }
}
