namespace BaseProject.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public OrderModel OrderModel { get; set; }

        public int ProductId { get; set; }
        public ProductModel ProductModel { get; set; }

        public int Quantity { get; set; }

    }
}
