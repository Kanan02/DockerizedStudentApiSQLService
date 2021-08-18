using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMicroservice.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime InsertedAt { get; set; }
    
    }
}
