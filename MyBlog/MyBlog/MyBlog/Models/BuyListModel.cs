using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class BuyListModel
    {
        [Key]
        public int Id { get; set; }

        public int Count { get; set; }

        public int ProductModelId { get; set; }
        public ProductModel ProductModel { get; set; }

        public string NewIdentityUserId { get; set; }
        public NewIdentityUser NewIdentityUser { get; set; }
    }
}
