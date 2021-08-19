using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
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
        private readonly IRabbitMQClient _rabbitMqClient;
        public StudentsController(StudentContext context, IRabbitMQClient rabbitMQClient)
        {
            _context = context;
            _rabbitMqClient = rabbitMQClient;
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
                var payload = JsonSerializer.Serialize(item);
                _rabbitMqClient.Publish("creating", "student.created", payload);
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
