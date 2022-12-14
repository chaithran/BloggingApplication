using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace BloggingAPI.Models
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int BlogId { get; internal set; }
        [Required(ErrorMessage = "Blog name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
    }
    public class BlogPost
    {       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get;internal set; }
        [Required,MinLength(1,ErrorMessage = "Content should have atleast one character")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int BlogId { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }

    }
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; internal set; }

        [Required, MinLength(1, ErrorMessage = "Comments should have atleast one character")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; internal set; } = DateTime.Now;
        [Required]
        public int PostId { get; set; }

    }
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagId { get; private set; }

        [DefaultValue("Welcome Christmas")]
        public string Name { get; set; }

    }
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; private set; }


        [Required, StringLength(30,MinimumLength =3, ErrorMessage = "User name must contain minimum 3 characters and upto 30 characters")]
        [DefaultValue("Chaithra")]
        public string Name { get; set; }


        [Required(ErrorMessage = "UserName is required")]
        [DefaultValue("chaithra.ediga")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [DefaultValue("test123@Test")]
        public string Password { get; set; }
    }



}
