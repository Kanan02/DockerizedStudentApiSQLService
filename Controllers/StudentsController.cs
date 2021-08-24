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
        private readonly IStudentService _context;
        private readonly IRabbitMQClient _rabbitMqClient;
        public StudentsController(IStudentService context, IRabbitMQClient rabbitMQClient)
        {
            _context = context;
            _rabbitMqClient = rabbitMQClient;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            var items= _context.GetStudents();
            return Ok(items);
        }

        [HttpGet("{id}", Name = "GetById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Student), (int)HttpStatusCode.OK)]
        [ActionName(nameof(GetById))]
        public ActionResult<Student> GetById(long id)
        {
            
            var item = _context.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        [HttpPost]
        [ProducesResponseType(typeof(Student), (int)HttpStatusCode.OK)]
        public ActionResult Create([FromBody] Student item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newItem = _context.Create(item);
            var payload = JsonSerializer.Serialize(newItem);
            _rabbitMqClient.Publish("creating", "student.created", payload);
            return CreatedAtAction("GetById", new { id = newItem.Id }, newItem);
  
           
           
        }
        [HttpPut("{id}")]
        public ActionResult Update(long id, [FromBody] Student item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newItem = _context.Update(id,item);
            
            var payload = JsonSerializer.Serialize(newItem);
            _rabbitMqClient.Publish("updating", "student.updated", payload);
            return Ok(newItem);
        }

        // DELETE api/shoppingcart/5
        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            var existingItem = _context.GetById(id);
            if (existingItem == null)
            {
                return NotFound();
            }
            _context.Delete(id);
            var payload = JsonSerializer.Serialize(id);
            _rabbitMqClient.Publish("deleting", "student.deleted", payload);
            return Ok();
        }

       
    }
}
