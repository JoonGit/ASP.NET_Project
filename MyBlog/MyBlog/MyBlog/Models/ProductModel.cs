using Microsoft.AspNetCore.Identity;
using MyBlog.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class ProductModel : IEntityBase
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }

        public string URI { get; set; }

        public string Document { get; set; }

        // 등록한 사람 정보
        public string RegisterUserId { get; set; }
        public NewIdentityUser RegisterUser { get; set; }

        // 구매 정보
        public List<BuyListModel> buyListModels { get; } = new List<BuyListModel>();
        // 찜하기 한 사람 정보
        public List<NewIdentityUser> WishUsers { get; } = new List<NewIdentityUser>();
    }
}
