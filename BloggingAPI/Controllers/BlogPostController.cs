using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloggingAPI.Data;
using BloggingAPI.Models;
using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.ObjectModel;

namespace BloggingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly BloggingAPIContext _context;

        public BlogPostsController(BloggingAPIContext context)
        {
            _context = context;
        }

        // GET: api/BlogPosts
        [HttpGet]
        [SwaggerOperation("Get all blog posts created")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPost()
        {
            return await _context.BlogPost.Include(x => x.Comments ?? new Collection<Comment>())
                                          .Include(x => x.Tags ?? new Collection<Tag>())
                                          .ToListAsync();
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        private async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = await _context.BlogPost.Include(x => x.Comments).Include(x => x.Tags).Include(x => x.Blog).FirstOrDefaultAsync(x => x.PostId == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        // PUT: api/BlogPosts/5
        [HttpPut("{id}/{modifierId}")]
        [SwaggerOperation("Edit own blog post")]
        public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost, int modifierId)
        {
            if (id != blogPost.PostId && modifierId != _context.Entry(blogPost).Entity.Blog.Owner.UserId)
            {
                //Blog Post doesnot exist or User not havin permission to blogPost
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
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

        // POST: api/BlogPosts
        [HttpPost]
        [SwaggerOperation("Create new blog post")]

        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            User user = await _context.User.FindAsync(blogPost.Blog.UserId);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }
            _context.BlogPost.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogPost", new { id = blogPost.PostId }, blogPost);
        }

        // DELETE: api/BlogPosts/5

        [HttpDelete("{id}/{modifierId}")]
        [SwaggerOperation("Delete own blog post")]

        public async Task<IActionResult> DeleteBlogPost(int id,int modifierId)
        {
            var blogPost = await _context.BlogPost.FirstOrDefaultAsync(x=>x.PostId == id && x.Blog.Owner.UserId == modifierId);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPost.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPost.Any(e => e.PostId == id);
        }
    }
}
