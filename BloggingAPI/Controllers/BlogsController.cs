using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloggingAPI.Data;
using BloggingAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace BloggingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly BloggingAPIContext _context;

        public BlogsController(BloggingAPIContext context)
        {
            _context = context;
        }

        public BlogsController()
        {
        }

        // GET: api/Blogs
        [HttpGet]
        [SwaggerOperation("List all Blogs")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlog()
        {
            return await _context.Blog.ToListAsync();
        }


        // GET: api/Blogs/5
        [HttpGet("{id}")]
        private async Task<ActionResult<Blog>> GetBlog(int id)
        {
            var blog = await _context.Blog.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            return blog;
        }


        // PUT: api/Blogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [SwaggerOperation("Update Blog")]
        public async Task<IActionResult> PutBlog(int id, Blog blog)
        {
            if (_context.Blog.Any(e => e.BlogId != id) || _context.User.Any(e => e.UserId != blog.UserId))
            {
                return BadRequest("BlogId or UserId is incorrect");
            }
            blog.BlogId = id;
            _context.Entry(blog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Blogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [SwaggerOperation("Create new Blog")]
        public async Task<ActionResult<Blog>> PostBlog(Blog blog)
        {
            var user = await _context.User.FindAsync(blog.UserId);
            if (user == null)
            {
            return NotFound("User not found");
            }
            _context.Blog.Add(blog);
            await _context.SaveChangesAsync();

            return Ok(blog.BlogId);
            //CreatedAtAction("GetBlog", new { id = blog.BlogId }, blog);
        }

        // DELETE: api/Blogs/5
        [HttpDelete("{id}")]
        [SwaggerOperation("Delete Blog")]

        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _context.Blog.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            _context.Blog.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogExists(int id)
        {
            return _context.Blog.Any(e => e.BlogId == id);
        }
    }
}
