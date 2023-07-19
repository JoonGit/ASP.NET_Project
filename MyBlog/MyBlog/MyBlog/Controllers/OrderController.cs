using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data.Cart;
using MyBlog.Data.Service;
using MyBlog.Data.ViewModels;
using MyBlog.Models;
using System.Security.Claims;

namespace MyBlog.Controllers
{
    public class OrderController : Controller
    {
        // GET: HomeController1
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrderService _ordersService;

        public OrderController(ShoppingCart shoppingCart, IOrderService orderService)
        {
            _shoppingCart = shoppingCart;
            _ordersService = orderService;
        }
        public async Task<ActionResult> Index()
        {
            // 구매 목록 가져오기
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            // view 아직 안만듬
            return View(orders);
        }

        public IActionResult ShoppingCart()
        {
            // 장바구니 생성
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(response);
        }



        #region 장바구니
        //[HttpGet("buy/{id:int}")]
        //public async Task<IActionResult> buy(int id)
        //{
        //    var user = HttpContext.User;
        //    // 현재 유저의 구매 이력 만들기 
        //    var curUser = await _userManager.GetUserAsync(user);
        //    BuyListModel buyList = new BuyListModel()
        //    {
        //        // 현재 구매하는 상품의 id
        //        ProductModelId = id,
        //        // 구매하는 유저의 id
        //        NewIdentityUserId = curUser.Id,
        //    };
        //    _dbContext.Add(buyList);
        //    _dbContext.SaveChanges();
        //    return Redirect("/product/list");
        //}
        #endregion
    }
}
