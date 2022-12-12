using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloggingAPI.Models
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Blog name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage ="Owner is not assigned for the Blog")]
        public User Owner { get; set; }
    }
    public class BlogPost : Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required,MinLength(1,ErrorMessage = "Content should have atleast one character")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public Comment[]? Comments { get; set; }
        public Tag[]? Tags { get; set; }
    }

    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MinLength(1, ErrorMessage = "Comments should have atleast one character")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required, StringLength(30,MinimumLength =3, ErrorMessage = "User name must contain minimum 3 characters and upto 30 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

}
