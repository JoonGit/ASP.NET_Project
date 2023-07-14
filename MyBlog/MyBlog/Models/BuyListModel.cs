namespace MyBlog.Models
{
    public class BuyListModel
    {
        public int Id { get; set; }

        public int ProductModelId { get; set; }
        public ProductModel ProductModel { get; set; }

        public string NewIdentityUserId { get; set; }
        public NewIdentityUser NewIdentityUser { get; set; }
    }
}
