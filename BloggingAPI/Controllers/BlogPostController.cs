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
using BloggingAPI.Models.DTOs;
using System.Configuration;
using System.Net.Http;
using System.Reflection.Metadata;

namespace BloggingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly BloggingAPIContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public BlogPostsController(BloggingAPIContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        // GET: api/BlogPosts
        [HttpGet]
        [SwaggerOperation("Get all blog posts created")]
        public async Task<ActionResult<IEnumerable<BlogPostDTOGet>>> GetBlogPost()
        {
            IEnumerable<BlogPostDTOGet> posts;

            var comments = _context.BlogPost.Join(_context.Comment,
               bp => bp.PostId,
               c => c.PostId,
               (bp, c) => new { bp, c }).Select(m => new
               {
                   Comments = m.c,
                   BlogPostId = m.bp.PostId
               });

            //var tags = _context.Tag.Join(_context.BlogPost,
            //  bp => bp.TagId
            //  t => t.TagIds,
            //  (bp, t) => new { bp, t }).Select(m => new
            //  {
            //      Tags = m.t,
            //      BlogPostId = m.bp.PostId
            //  });

            var TagIds = _context.BlogPost.Select(x => x.TagIds);
            var tags = new List<Tag>();
            foreach (var item in TagIds)
            {
                Tag tag = _context.Tag.Where(x => x.TagId.Equals(item)).FirstOrDefault();
                tags.Add(tag);
            }

            var blog_user = _context.BlogPost.Join(_context.Blog,
               b => b.BlogId,
               bp => bp.BlogId,
               (bp, b) => new { b, bp }).Join(_context.User,
               bbp => bbp.b.UserId,
               u => u.UserId,
               (bbp, u) => new { bbp, u }).Select(m => new
               {
                   BlogName = m.bbp.b.Name,
                   BlogDescription = m.bbp.b.Description,
                   UserName = m.u.Name,
                   BlogPostId = m.bbp.bp.PostId,
                   BlogId = m.bbp.b.BlogId,
                   UserId = m.u.UserId,
                   PostId=m.bbp.bp.PostId,
               });

            Collection<BlogPostDTOGet> blogPostDTOs= new Collection<BlogPostDTOGet>();

            foreach(var blogPost in _context.BlogPost)
            {
                var postDTO = new BlogPostDTOGet();
                postDTO.BlogName = blog_user.Where(x => x.BlogPostId.Equals(blogPost.PostId))
                                            .Select(x => x.BlogName)
                                            .FirstOrDefault();
                postDTO.BlogDescription = blog_user.Where(x => x.BlogPostId.Equals(blogPost.PostId)).Select(x => x.BlogDescription).FirstOrDefault();
                postDTO.BlogId = blog_user.Where(x => x.BlogPostId.Equals(blogPost.PostId)).Select(x => x.BlogId).FirstOrDefault();
                postDTO.BlogId = blog_user.Where(x => x.BlogPostId.Equals(blogPost.PostId)).Select(x => x.UserId).FirstOrDefault();
                postDTO.UserName = blog_user.Where(x => x.BlogPostId.Equals(blogPost.PostId)).Select(x => x.UserName).FirstOrDefault();
                postDTO.PostId = blog_user.Where(x => x.BlogPostId.Equals(blogPost.PostId)).Select(x => x.PostId).FirstOrDefault();

                postDTO.Content = blogPost.Content;
                postDTO.CreatedDate = blogPost.CreatedDate;
                postDTO.Tags = tags;
                postDTO.Comments = comments.Where(x => x.BlogPostId.Equals(blogPost.PostId)).Select(x => x.Comments).ToList();
                blogPostDTOs.Add(postDTO);
            }
            return blogPostDTOs;

            //var TagIds = _context.BlogPost.Select(x => x.TagIds).ToListAsync();
            //var names = new Collection<Tag>();
            //foreach(var item in TagIds.Result) 
            //{ 
            //    Tag name = _context.Tag.Where(x=>x.TagId.Equals(item)).FirstOrDefault();
            //    names.Add(name);
            //}

            //var CommentIds = _context.BlogPost.Select(x => x.CommentIds).ToListAsync();
            //var comments = new Collection<Comment>();
            //foreach (var item in CommentIds.Result)
            //{
            //    Comment comment = _context.Comment.Where(x => x.CommentId.Equals(item)).FirstOrDefault();
            //    comments.Add(comment);
            //}

            //return await _context.BlogPost.Join(_context.Blog,
            //   b => b.BlogId,
            //   bp => bp.BlogId,
            //   (bp, b) => new { b, bp }).Join(_context.User,
            //   bbp => bbp.b.UserId,
            //   u => u.UserId,
            //   (bbp, u) => new { bbp, u }).
               
               
               
               //Select(m => new BlogPostDTO
               //{
               //    BlogName = m.bbp.b.Name,
               //    BlogDescription = m.bbp.b.Description,
               //    Content = m.bbp.bp.Content,
               //    CreatedDate = m.bbp.bp.CreatedDate,
               //    UserName = m.u.Name,
               //    Tags = names,
               //    Comments = comments
               //}).ToListAsync();

        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        private async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = new BlogPost();
            //await _context.BlogPost.Include(x => x.Comments).Include(x => x.Tags).Include(x => x.Blog).FirstOrDefaultAsync(x => x.PostId == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        // PUT: api/BlogPosts/5
        [HttpPut("{id}/{modifierId}")]
        [SwaggerOperation("Edit own blog post")]
        public async Task<IActionResult> PutBlogPost(int id, int modifierId, BlogPostDTOEdit blogPostDto)
        {
            if (_context.BlogPost.Any(e => e.PostId != id))
            {
                return BadRequest("BlogPostId is incorrect");
            }
            var blogId = _context.BlogPost.Where(x => x.PostId == id).Select(x => x.BlogId).FirstOrDefault();
            bool doesUserHasAccess = _context.Blog.Where(x => x.BlogId.Equals(blogId)).Select(x => x.UserId).FirstOrDefault().Equals(modifierId);
            if (!doesUserHasAccess)
            {
                return BadRequest("Modifier does not have permission!");
            }
            BlogPost blogPost = _context.BlogPost.Where(x => x.PostId == id).FirstOrDefault();
            if (blogPost != null)
            {
                blogPost.Content = blogPostDto.Content ?? blogPost.Content;
                blogPost.TagIds = blogPostDto.Tags ?? blogPost.TagIds;
            }
            Blog blog = _context.Blog.Where(x => x.BlogId == blogId).FirstOrDefault();
            if (blog != null)
            {
                blog.Name = blogPostDto.BlogName ?? blog.Name;
                blog.Description = blogPostDto.BlogDescription ?? blog.Description;
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
        public async Task<ActionResult<BlogPostDTOPost>> PostBlogPostDTO(BlogPostDTOPost blogPostDTO)
        {
            /*
             There are 2 ways to create Blogs, one is with blogPost and without or empty blogPost,
             hence there is a possibility to pass existing blogId
             */
            Blog blog = await _context.Blog.FindAsync(blogPostDTO.BlogId);
            int blogId=blogPostDTO.BlogId;
            
            if (blog == null)
            {
                var user = await _context.User.FindAsync(blogPostDTO.UserId);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                var res=_context.Blog.Add(new Blog { Name = blogPostDTO.BlogName, Description = blogPostDTO.BlogDescription, UserId = blogPostDTO.UserId });
                
                blogId = res.Entity.BlogId;
            }
            foreach (var item in blogPostDTO.Tags)
            {
                if (item!=0 && !_context.Tag.Any(x => x.TagId.Equals(item)))
                    return BadRequest("Tag does not exist");
            }
            BlogPost blogPost = new BlogPost
            {
                BlogId = blogId,
                Content = blogPostDTO.Content,
                TagIds =  blogPostDTO.Tags
            };
            _context.BlogPost.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogPost", new { id = blogPost.PostId }, blogPost);
        }
        //private async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        //{
        //    Blog blog = await _context.Blog.FindAsync(blogPost.BlogId);
        //    if (blog == null)
        //    {
        //        return BadRequest("Blog does not exist");
        //    }
        //    foreach (var item in blogPost.TagIds)
        //    {
        //        if (_context.Tag.Any(x => x.TagId.Equals(item)))
        //            return BadRequest("Tag does not exist");
        //    }
            
        //    _context.BlogPost.Add(blogPost);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetBlogPost", new { id = blogPost.PostId }, blogPost);
        //}

        // DELETE: api/BlogPosts/5

        [HttpDelete("{id}/{modifierId}")]
        [SwaggerOperation("Delete own blog post")]

        public async Task<IActionResult> DeleteBlogPost(int id,int modifierId)
        {
            if (_context.BlogPost.Any(e => e.PostId != id))
            {
                return BadRequest("BlogPostId is incorrect");
            }
            var blogId = _context.BlogPost.Where(x => x.PostId == id).Select(x => x.BlogId).FirstOrDefault();
            bool doesUserHasAccess = _context.Blog.Where(x => x.BlogId.Equals(blogId)).Select(x => x.UserId).FirstOrDefault().Equals(modifierId);
            if (!doesUserHasAccess)
            {
                return BadRequest("Modifier does not have permission!");
            }
            BlogPost blogPost = _context.BlogPost.Where(x => x.PostId == id).FirstOrDefault();
            _context.BlogPost.Remove(blogPost);

            var comments = _context.Comment.Where(x => x.PostId == id).ToList();
            if (comments.Count > 0)
            {
                foreach(var comment in comments)
                    _context.Comment.Remove(comment);
            }           

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPost.Any(e => e.PostId == id);
        }
    }
}
