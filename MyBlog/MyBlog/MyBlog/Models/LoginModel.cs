using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class LoginModel
    {
        [Required]
        public string id { get; set; }

        [Required] 
        public string password { get; set; }
    }
}
