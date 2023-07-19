using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class ShoppingCartItem
    {

        [Key]
        public int Id { get; set; }

        public ProductModel Product { get; set; }
        public int Count { get; set; }


        public string ShoppingCartId { get; set; }
    }
}
