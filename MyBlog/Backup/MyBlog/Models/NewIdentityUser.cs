using Microsoft.AspNetCore.Identity;

namespace MyBlog.Models
{
    public class NewIdentityUser : IdentityUser
    {
        public List<ProductModel> RegisterModels { get; }
            = new List<ProductModel>();
        public List<BuyListModel> buyListModels { get; } = new List<BuyListModel>();
        public List<ProductModel> WishProducts { get; }
            = new List<ProductModel>();

    }
}
