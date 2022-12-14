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
    public class TagsController : ControllerBase
    {
        private readonly BloggingAPIContext _context;

        public TagsController(BloggingAPIContext context)
        {
            _context = context;
        }

        // GET: api/Tags
        [HttpGet]
        [SwaggerOperation("List tags")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTag()
        {
            return await _context.Tag.ToListAsync();
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        private async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag = await _context.Tag.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }    

        // POST: api/Tags
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [SwaggerOperation("Create new tag")]
        public async Task<ActionResult<Tag>> PostTag(Tag tag)
        {
            _context.Tag.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.TagId }, tag);
        }

    }
}
