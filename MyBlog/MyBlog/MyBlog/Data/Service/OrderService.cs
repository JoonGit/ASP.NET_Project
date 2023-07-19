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

        private async Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress)
        {
            var order = new OrderModel()
            {
                UserId = userId,
                Email = userEmailAddress
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItemModel()
                {
                    Count = item.Count,
                    MovieId = item.Product.Id,
                    OrderId = order.Id,
                    Price = item.Product.Price
                };
                await _context.OrderItems.AddAsync(orderItem);
            }
            await _context.SaveChangesAsync();
        }

        Task<List<OrderModel>> IOrderService.GetOrdersByUserIdAndRoleAsync(string userId, string userRole)
        {
            return GetOrdersByUserIdAndRoleAsync(userId, userRole);
        }

        Task IOrderService.StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress)
        {
            return StoreOrderAsync(items, userId, userEmailAddress);
        }
    }
}
