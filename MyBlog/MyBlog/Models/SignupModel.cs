using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class SignupModel
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string password { get; set; }
    }
}
