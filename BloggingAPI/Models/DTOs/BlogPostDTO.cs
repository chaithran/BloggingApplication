using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BloggingAPI.Models.DTOs
{
    public class BlogPostDTOGet
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BlogName { get; set; }
        public int BlogId { get; internal set; }
        public string BlogDescription { get; set; }
        public string UserName { get; set; }
        public int UserId { get; internal set; }
        public ICollection<Comment> Comments { get; set; }
        public List<Tag> Tags { get; set; }
    }
    public class BlogPostDTOPost
    {
        public string Content { get; set; }
        public int BlogId { get;internal set; }
        public string BlogName { get; set; }
        public string BlogDescription { get; set; }
        public int UserId { get; set; }
        public List<int> Tags { get; set; }
    }
    public class BlogPostDTOEdit
    {
        public string? Content { get; set; }
        public string? BlogName { get; set; }
        public string? BlogDescription { get; set; }
        public List<int>? Tags { get; set; }
    }
}
