using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BloggingAPI.Models;

namespace BloggingAPI.Data
{
    public class BloggingAPIContext : DbContext
    {
        public BloggingAPIContext (DbContextOptions<BloggingAPIContext> options)
            : base(options)
        {
        }

        public DbSet<BloggingAPI.Models.User> User { get; set; } = default!;

        public DbSet<BloggingAPI.Models.Comment> Comment { get; set; }

        public DbSet<BloggingAPI.Models.BlogPost> BlogPost { get; set; }

        public DbSet<BloggingAPI.Models.Blog> Blog { get; set; }
    }
}
