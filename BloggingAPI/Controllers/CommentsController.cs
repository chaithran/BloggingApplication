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
    public class CommentsController : ControllerBase
    {
        private readonly BloggingAPIContext _context;

        public CommentsController(BloggingAPIContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        [SwaggerOperation("Get List of Comments")]

        public async Task<ActionResult<IEnumerable<Comment>>> GetComment()
        {
            return await _context.Comment.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        private async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comment.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [SwaggerOperation("Edit Comment")]

        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (_context.Comment.Any(e => e.CommentId != id) || _context.BlogPost.Any(e => e.PostId == comment.PostId))
            {
                return BadRequest("PostId or CommentId is incorrect");
            }
            comment.CommentId = id;
            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [SwaggerOperation("Insert Comment")]

        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            var post = await _context.BlogPost.FindAsync(comment.PostId);
            if (post == null)
            {
                return NotFound("Blog Post not found or PostID is incorrect");
            }
            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.CommentId }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        [SwaggerOperation("Delete Comment")]

        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.CommentId == id);
        }
    }
}
