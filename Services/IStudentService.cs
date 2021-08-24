using MyMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMicroservice.Services
{
    public interface IStudentService
    {
        public IEnumerable<Student> GetStudents();
        public Student GetById(long id);
        public Student Create(Student item);
        public Student Update(long id, Student item);
        public void Delete(long id);
    }
}
