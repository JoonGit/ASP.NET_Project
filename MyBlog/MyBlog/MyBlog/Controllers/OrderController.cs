﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MyBlog.Data;
using MyBlog.Data.Cart;
using MyBlog.Data.Service;
using MyBlog.Data.ViewModels;
using MyBlog.Models;
using System.Security.Claims;
using System.Drawing.Printing;

namespace MyBlog.Controllers
{
    [Route("order")]
    public class OrderController : Controller
    {
        // GET: HomeController1
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrderService _ordersService;
        private readonly IProudctesService _proudctesService;
        private readonly UserManager<NewIdentityUser> _userManager;
        private readonly BlogDbContext _dbContext;

        public OrderController(ShoppingCart shoppingCart
            , IOrderService orderService
            , IProudctesService proudctesService
            , UserManager<NewIdentityUser> userManager
            , BlogDbContext dbContext)
            
        {
            _shoppingCart = shoppingCart;
            _ordersService = orderService;
            _proudctesService = proudctesService;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            Console.WriteLine("GetShoppingCart 실행");
            // 현재 접속한 유저의 DB를 가져옴
            var context = services.GetService<BlogDbContext>();

            // 현재 접속한 유저의 Session에 CartId가 없으면 새로 생성
            //string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            string cartId = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Request.Cookies["CartId"] ?? Guid.NewGuid().ToString();
            // 현재 접속한 유저의 Session에 CartId를 저장
            var identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim("ItemCount", Convert.ToString(0)), // 저장하고 싶은 사용자 명
                        new Claim("CartId", "ItemCount"), // 저장하고 싶은 사용자 명
                    }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

            services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: new ClaimsPrincipal(identity: identity),
                properties: new AuthenticationProperties()
                );

            // 현재 접속한 유저의 DB에 CartId가 없으면 새로 생성
            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        #region 구매목록
        [HttpGet("index")]
        public async Task<ActionResult> Index(string returnUrl)
        {
            // 구매 목록 가져오기
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            ViewBag.returnUrl = returnUrl;
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

            var resp = new HttpResponseMessage();


            if (item != null)
            {
                _shoppingCart.AddItemToCart(item, count);


            }
            return Redirect("/order/ShoppingCart");
            //return RedirectToAction(nameof(OrderController.ShoppingCart));
        }

        #endregion

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
        [HttpGet("clear")]
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

        #region 구매
        // 임시로 Get
        [HttpGet("buy")]
        public async Task<IActionResult> buy()
        {
            var user = HttpContext.User;
            // 현재 유저의 구매 이력 만들기 
            var curUser = await _userManager.GetUserAsync(user);
            var items = _shoppingCart.GetShoppingCartItems();
            foreach (var item in items)
            {
                BuyListModel buyList = new BuyListModel()
                {
                    // 현재 구매하는 상품의 id
                    ProductModel = item.Product,
                    ProductModelId = item.Product.Id,
                    // 구매하는 유저의 id
                    NewIdentityUser = curUser,
                    NewIdentityUserId = curUser.Id,
                    Count = item.Count,
                };
                _dbContext.BuyLists.Add(buyList);
            }
            // 결제후 db 저장
            _dbContext.SaveChanges();
            CompleteOrder();
            return Redirect("/product/list");
        }
        #endregion
    }
}
