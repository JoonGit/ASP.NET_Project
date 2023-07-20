using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Data.Service;
using MyBlog.Data.Static;
using MyBlog.Models;
using System.Linq;
using System.Security.Claims;

namespace MyBlog.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _usersService;
        private readonly UserManager<NewIdentityUser> _userManager;
        private readonly SignInManager<NewIdentityUser> _signInManager;
        private readonly BlogDbContext _dbContext;

        public AccountController(
            IAccountService usersService
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
        public async Task<IActionResult> Signup(string role)
        {
            ViewBag.role = role;
            return View();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupModel model, string role)
        {

            if (!ModelState.IsValid) return View(model);
            var newUser = new NewIdentityUser { UserName = model.name, Email = model.email};
            var newUserResponse = await _userManager.CreateAsync(newUser, model.password);
            if (newUserResponse.Succeeded)
            {
                if(role.Equals(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                }
                else if (role.Equals(UserRoles.Seller))
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Seller);
                }
                return Redirect("/account/login");
            }
            TempData["Error"] = "Wrong credentials. Please, try again!";
            ViewBag.role = role;
            return View(model);
        }
        #endregion

        #region 로그인
        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _signInManager.PasswordSignInAsync(model.id, model.password, false, false);
            if (user.Succeeded)
            {
                if(ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return Redirect("/");
                }
                
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


        #region 권한 없는 사용자가 접근했을 때 보내지는 페이지
        [HttpGet("accessdenied")]
        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }
        #endregion
    }
}
