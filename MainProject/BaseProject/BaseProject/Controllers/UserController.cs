﻿using BaseProject.Data;
using BaseProject.Data.Service;
using BaseProject.Data.Static;
using BaseProject.Migrations;
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
        private readonly IFileService _fileService;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;
        private readonly BaseDbContext _dbContext;

        public UserController(
            IFileService fileService
            , UserManager<UserIdentity> userManager
            , SignInManager<UserIdentity> signInManager
            , BaseDbContext dbContext)
        {
            _fileService = fileService;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        #region 회원가입
        [HttpGet("create")]
        public async Task<IActionResult> CreateUser(string role)
        {
            ViewBag.role = role;
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(Register_Model model,  IFormFile file)
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
            newUser.ImgUrl = await _fileService.FileCreat(newUser.Id, file, "user");

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
        public async Task<IActionResult> RollAccept(RollAccept_Model rollAccept_Model)
        {
            for (int i = 0; i< rollAccept_Model.UserId.Length; i++)
            {
                var result = await _userManager.RemoveFromRoleAsync(await _userManager.FindByIdAsync(rollAccept_Model.UserId[i]), rollAccept_Model.BeforeRole[i]);
                if (result.Succeeded)
                {
                    if (rollAccept_Model.Role[i].Equals(UserRoles.Member))
                    {
                        await _userManager.AddToRoleAsync(await _userManager.FindByIdAsync(rollAccept_Model.UserId[i]), UserRoles.Member);
                    }
                    else if (rollAccept_Model.Role[i].Equals(UserRoles.Manager))
                    {
                        await _userManager.AddToRoleAsync(await _userManager.FindByIdAsync(rollAccept_Model.UserId[i]), UserRoles.Manager);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(await _userManager.FindByIdAsync(rollAccept_Model.UserId[i]), UserRoles.NoRole);
                    }
                }
            }
            
            
            return Redirect("/user/login");
        }

        #endregion

        #region 마이페이지
        [HttpGet("UpdateUser")]
        public async Task<IActionResult> UpdateUser()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.Name);
            return View(user);
        }
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserIdentity user, string Password, IFormFile file)
        {
            // 유저 정보 수정

            var updateUser = await _userManager.FindByIdAsync(User.Identity.Name);
            updateUser.UserName = user.UserName;

            // 비밀번호 변경
            var token = await _userManager.GeneratePasswordResetTokenAsync(updateUser);
            var result = await _userManager.ResetPasswordAsync(updateUser, token, Password);

            updateUser.Status = user.Status;
            updateUser.ImgUrl = await _fileService.FileUpdate(updateUser.Id, file, "user");

            _userManager.UpdateAsync(updateUser);
            User_Edit_Log_Model user_Edit_Log_Model = new User_Edit_Log_Model()
            {
                UserIdentityId = updateUser.Id,
                EditTime = DateTime.Now
            };
            _dbContext.User_Edit_Log_Models.Add(user_Edit_Log_Model);            
            _dbContext.SaveChanges();
            return View(user);
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
        public async Task<IActionResult> Login(Login_Model model, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            var loginUser = await _userManager.FindByIdAsync(model.Id);

            var user = await _signInManager.PasswordSignInAsync(loginUser, model.Password, false, false);
            if (user.Succeeded)
            {
                Login_Log_Model login_Log_Model = new Login_Log_Model()
                {
                    UserId = model.Id,
                    LoginTime = DateTime.Now
                };
                _dbContext.SaveChanges();
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
