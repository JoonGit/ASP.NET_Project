using MyBlog.Models;

namespace MyBlog.Data.Service
{
    public interface IOrderService
    {
        Task<List<OrderModel>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole);
    }
}
