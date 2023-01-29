using Adapter.BlogApi.Services.Interfaces;
using Adapter.BlogApi.Services.Models;
using Adapter.BlogApi.Services.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Adapter.BlogApi.Services.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [EnableCors()]
    [Authorize]
    public class BlogsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IRabitMQProducer _rabitMQProducer;
        public BlogsController(
            ApplicationContext context,
            IRabitMQProducer rabitMQProducer
            )
        {
            _context = context;
            _rabitMQProducer = rabitMQProducer;
        }
        /// <summary>
        /// Get list of blogs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Blogs.ToListAsync());
        }
        /// <summary>
        /// Get blog by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _context.Blogs.FindAsync(id);

            if (response == null)
                return NotFound();

            return Ok(response);
        }
        /// <summary>
        /// Create a new blog
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Blog request)
        {
            if (request == null)
                return BadRequest(new { errorText = "Invalid request!" });

            try
            {
                var result = _context.Blogs.Add(request);

                await _context.SaveChangesAsync();

                _rabitMQProducer.SendProductMessage(result.Entity);

                return Ok(request);
            }
            catch
            {
                return BadRequest(new { errorText = "Posting data has been failed!" });
            }
        }
        /// <summary>
        /// Update blog by Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Blog request)
        {
            if (request == null)
                return BadRequest(new { errorText = "Invalid request!" });

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                _context.Update(request);

                await _context.SaveChangesAsync();

                return Ok(request);
            }
            catch
            {
                return BadRequest(new { errorText = "Editing data has been failed!" });
            }
        }
        /// <summary>
        /// Update blog with patch action
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Patch([FromBody] JsonPatchDocument request, [FromRoute] int id)
        {
            var response = _context.Blogs.Find(id);

            if (response == null)
                return NotFound();

            request.ApplyTo(response);

            await _context.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Delete blog by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = _context.Blogs.Find(id);

            if (response == null)
                return NotFound();

            try
            {
                _context.Blogs.Remove(response);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return BadRequest(new { errorText = "Deleting data has been failed!" });
            }
        }
    }
}
