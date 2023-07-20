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
    [Route("order")]
    public class OrderController : Controller
    {
        // GET: HomeController1
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrderService _ordersService;
        private readonly IProudctesService _proudctesService;

        public OrderController(ShoppingCart shoppingCart
            , IOrderService orderService
            , IProudctesService proudctesService)
        {
            _shoppingCart = shoppingCart;
            _ordersService = orderService;
            _proudctesService = proudctesService;
        }

        #region 구매목록
        public async Task<ActionResult> Index()
        {
            // 구매 목록 가져오기
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            // view 아직 안만듬
            return View(orders);
        }
        #endregion

        #region 장바구니 생성 및 가져오기
        [HttpGet("ShoppingCart")]
        public ActionResult ShoppingCart()
        {
            Console.WriteLine("장바구니");
            // 장바구니 생성
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };
            ViewBag.total = response.ShoppingCartTotal;
            //return "장바구니";
            return View(response);
        }
        #endregion

        #region 장바구니에 상품 추가
        [HttpPost("add")]
        // 구매시 상품 id와 count값 전달 받도록 만들기
        public async Task<IActionResult> AddItemToShoppingCart(int id, int count)
        {
            var item = await _proudctesService.GetByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.AddItemToCart(item, count);
            }
            return Redirect("/order/ShoppingCart");
            //return RedirectToAction(nameof(OrderController.ShoppingCart));
        }

        #endregion

        // 상품 수정과 상품 삭제 구분하기


        #region 장바구니에 상품 수정
        [HttpPost("edit")]
        public async Task<IActionResult> EditItemFromShoppingCart(int id, int count)
        {
            var item = await _proudctesService.GetByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.EditItemFromCart(item, count);
            }
            return Redirect("/order/ShoppingCart");
        }
        #endregion
        #region 장바구니에 상품 삭제
        [HttpGet("remove")]
        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await _proudctesService.GetByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(item);
            }
            return Redirect("/order/ShoppingCart");
        }
        #endregion

        #region 장바구니에 상품 전체 삭제
        [HttpPost("clear")]
        public async Task<IActionResult> CompleteOrder()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            await _ordersService.StoreOrderAsync(items, userId, userEmailAddress);
            await _shoppingCart.ClearShoppingCartAsync();

            return View("OrderCompleted");
        }
        #endregion

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
