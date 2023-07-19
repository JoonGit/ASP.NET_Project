using Microsoft.EntityFrameworkCore;
using MyBlog.Models;

namespace MyBlog.Data.Service
{
    public class OrderService : IOrderService
    {
        private readonly BlogDbContext _context;

        public OrderService(BlogDbContext context)
        {
            _context = context;
        }


        private async Task<List<OrderModel>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole)
        {
            var orders = await _context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Product).Include(n => n.User).ToListAsync();

            if (userRole != "Admin")
            {
                orders = orders.Where(n => n.UserId == userId).ToList();
            }

            return orders;
        }

        Task<List<OrderModel>> IOrderService.GetOrdersByUserIdAndRoleAsync(string userId, string userRole)
        {
            return GetOrdersByUserIdAndRoleAsync(userId, userRole);
        }
    }
}
