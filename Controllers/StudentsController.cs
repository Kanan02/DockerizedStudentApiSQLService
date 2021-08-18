using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyMicroservice.Models;
using MyMicroservice.Services;

namespace MyMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentContext _context;
        public StudentsController(StudentContext context)
        {
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            return _context.GetStudents();
        }

        [HttpGet("{id}", Name = "GetById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Student), (int)HttpStatusCode.OK)]
        [ActionName(nameof(GetById))]
        public IActionResult GetById(long id)
        {
            
            var item = _context.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Student), (int)HttpStatusCode.OK)]
        public IActionResult Create([FromBody] Student item)
        {
            if (_context.Create(item))
            {
                return CreatedAtRoute("GetById", new { id = item.Id }, item);
                
            }
            else
            {
                return BadRequest();
            }
           
        }
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Student item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }
            if (_context.Update(id,item))
            {
                return new NoContentResult();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            if (_context.Delete(id))
            {
                return new NoContentResult();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
