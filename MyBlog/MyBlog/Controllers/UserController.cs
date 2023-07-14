using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Data.Service;
using MyBlog.Models;
using System.Linq;
using System.Security.Claims;

namespace MyBlog.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<NewIdentityUser> _userManager;
        private readonly SignInManager<NewIdentityUser> _signInManager;
        private readonly BlogDbContext _dbContext;

        public UserController(
            IUsersService usersService
            , UserManager<NewIdentityUser> userManager
            ,SignInManager<NewIdentityUser> signInManager
            , BlogDbContext dbContext)
        {
            _usersService = usersService;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        #region 회원가입
        [HttpGet("signup")]
        public async Task<IActionResult> Signup()
        {
            return View();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupModel model)
        {

            if (!ModelState.IsValid) return View(model);
            var user = new NewIdentityUser { UserName = model.name, Email = model.email};
            var newUserResponse = await _userManager.CreateAsync(user, model.password);
            if (newUserResponse.Succeeded)
            {
                return Redirect("/user/login");
            }
            else
            {
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(model);
            }
                  
        }
        #endregion

        #region 로그인
        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _signInManager.PasswordSignInAsync(model.id, model.password, false, false);
            if (user.Succeeded)
            {
                return Redirect("/");
            }
            else
            {
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(model);
            }           
        }
        #endregion

        #region 로그아웃
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion

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
