using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBlog.Models
{
    public class OrderItemModel
    {
        [Key]
        public int Id { get; set; }

        public int Count { get; set; }
        public double Price { get; set; }

        public int MovieId { get; set; }
        [ForeignKey("ProductModelId")]
        public ProductModel Product { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public OrderModel Order { get; set; }
    }
}
