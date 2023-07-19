using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using System.Security.Claims;

namespace MyBlog.Controllers
{
    public class UserController : Controller
    {
        private readonly BlogDbContext _dbContext;

        public UserController(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region 구매내역
        [HttpGet("buylist")]
        public async Task<IActionResult> MyPage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _dbContext.BuyLists
                .Where(b => b.NewIdentityUserId == userId)
                .Include(b => b.ProductModel)
                .Select(b => b.ProductModel)
                .ToListAsync();
            int total = 0;
            foreach (var item in result)
            {
                total += item.Price;
            }
            ViewBag.tatal = total;

            return View(result);
        }
        #endregion
    }
}
