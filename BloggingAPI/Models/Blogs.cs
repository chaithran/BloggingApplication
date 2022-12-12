using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloggingAPI.Models
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogId { get;private set; }
        [Required(ErrorMessage = "Blog name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        //public virtual ICollection<BlogPost> BlogPosts { get; set; }
        [ForeignKey("UserId")]
        public User Owner { get; set; }
        public int UserId { get; set; }
    }
    public class BlogPost
    {       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; private set; }
        [Required,MinLength(1,ErrorMessage = "Content should have atleast one character")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int BlogId { get;private set; }
        public Blog Blog { get; set; }

        public virtual ICollection<Comment>? Comments { get;private set; }
        public virtual ICollection<Tag>? Tags { get;private set; }
    }

    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; private set; }

        [Required, MinLength(1, ErrorMessage = "Comments should have atleast one character")]
        public string Content { get; set; }
        public DateTime CreatedDate { get;private set; } = DateTime.Now;
        public int PostId { get; private set; }
        //public BlogPost BlogPost { get; set; }

        public int UserId { get; private set; }

    }
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public string Name { get; set; }
        //public ICollection<BlogPost> BlogPosts { get; set; }
    }
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; private set; }
        [Required, StringLength(30,MinimumLength =3, ErrorMessage = "User name must contain minimum 3 characters and upto 30 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //public ICollection<Blog> Blogs { get; } = new List<Blog>();
        //public ICollection<Comment> Comments { get; } = new List<Comment>();


    }

}
