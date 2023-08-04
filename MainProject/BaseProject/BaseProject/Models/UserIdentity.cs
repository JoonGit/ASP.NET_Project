using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.Models
{
    public class UserIdentity : IdentityUser
    {
        public string ImgUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
