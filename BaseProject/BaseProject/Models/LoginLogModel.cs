using BaseProject.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.Models
{
    public class LoginLogModel
    {
        [Key]
        public int Id { get; set; }

        public int UserIdentityId { get; set; }
        public UserIdentity UserIdentity { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
