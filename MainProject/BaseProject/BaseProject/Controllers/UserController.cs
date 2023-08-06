using BaseProject.Data;
using BaseProject.Data.Service;
using BaseProject.Data.Static;
using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BaseProject.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService _usersService;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;
        private readonly BaseDbContext _dbContext;

        public UserController(
            IUserService usersService
            , UserManager<UserIdentity> userManager
            , SignInManager<UserIdentity> signInManager
            , BaseDbContext dbContext)
        {
            _usersService = usersService;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        #region 회원가입
        [HttpGet("create")]
        public async Task<IActionResult> Signup(string role)
        {
            ViewBag.role = role;
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> SignUp(RegisterModel model,  IFormFile file)
        {
            // 유효성 검사
            //if (!ModelState.IsValid) return View(model);
            await Console.Out.WriteLineAsync("signup");

            // 유저 정보 입력
            var newUser = new UserIdentity
            {
                Id = model.Id,
                UserName = model.Name,
                ImgUrl = "/User/" + model.Id + "/" + file.FileName,
                Status = "True",
                CreateTime = DateTime.Now
            };

            // 유저 이미지 업로드
            newUser.ImgUrl = await _usersService.FileUpload(newUser, file);

            // 유저 생성
            if (newUser.ImgUrl != "Fail") 
            {
                var newUserResponse = await _userManager.CreateAsync(newUser, model.Password);
                if (newUserResponse.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.NoRole);

                    return Redirect("/user/login");
                }
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(model);
            }
            else
            {
                return Redirect("/user/create");
            }
        }
        #endregion

        #region 유저리스트
        [HttpGet("userlist")]
        public async Task<IActionResult> UserList()
        {

            // db에 있는 권한 정보 가져오기

            
            var userList = await (from userRole in _dbContext.UserRoles
                              join role in _dbContext.Roles on userRole.RoleId equals role.Id
                              join user in _dbContext.Users on userRole.UserId equals user.Id
                              select new
                              {
                                  Id = user.Id,
                                  Role = role.Name,
                                  UserName = user.UserName,
                                  ImgUrl = user.ImgUrl
                              }).ToListAsync();

            return View(userList);
        }

        #endregion
        #region 권한 승인
        [HttpPost("rollaccept")]
        public async Task<IActionResult> RollAccept(string Id, string Role, string BeforeRole)
        {
            var result = await _userManager.RemoveFromRoleAsync(await _userManager.FindByIdAsync(Id), BeforeRole);
            if (result.Succeeded)
            {
                if (Role.Equals(UserRoles.Member))
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByIdAsync(Id), UserRoles.Member);
                }
                else if (Role.Equals(UserRoles.Manager))
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByIdAsync(Id), UserRoles.Manager);
                }
                else
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByIdAsync(Id), UserRoles.NoRole);
                }
            }
            
            return Redirect("/user/login");
        }

        #endregion

        #region 로그인
        [HttpGet("login")]
        public async Task<IActionResult> Login(string ReturnUrl)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            var loginUser = await _userManager.FindByIdAsync(model.Id);

            var user = await _signInManager.PasswordSignInAsync(loginUser, model.Password, false, false);
            if (user.Succeeded)
            {
                if (ReturnUrl != null)
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

        #region 회원탈퇴
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.Status = "False";
            await _userManager.UpdateAsync(user);
            return Redirect("/user/login");
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
