using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Data.Service;
using MyBlog.Data.Static;
using MyBlog.Models;

namespace MyBlog.Controllers
{
    //[Authorize(Roles = UserRoles.Admin)]
    [Authorize(Roles = UserRoles.Seller)]
    [Route("Product")]
    public class ProductController : Controller
    {

        private readonly IProudctesService _service;
        private readonly UserManager<NewIdentityUser> _userManager;
        private readonly BlogDbContext _dbContext;
        private readonly IFileService _fileService;


        public ProductController(
            IProudctesService service
            , UserManager<NewIdentityUser> userManager
            , BlogDbContext dbContext
            , IFileService fileService
            )
        {
            _service = service;
            _userManager = userManager;
            _dbContext = dbContext;
            _fileService = fileService;
        }

        #region 상품등록
        [HttpGet("register")]
        public async Task<IActionResult> Register()
        {
            // 사용자 정보는 나중에 캐시나 세션으로 값 받아오는 걸로 해결
            ViewBag.userId =_userManager.GetUserId(HttpContext.User);
            return View();
        }

        // 권한을 소비자 등록자 로 나워 등록자만 접근 가능하도록 변경
        [HttpPost("register")]
        public async Task<IActionResult> Register(ProductModel model, IFormFileCollection files)
        {
            // 파일 저장
            if (await _service.Upload(model, files) > 0) { return View(model); }

            // 파일 업로드후 모델 저장
            await _service.AddAsync(model);
            return Redirect("/");
        }
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        #endregion

        #region 상품목록조회
        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            // 전체 상품 목록 조회
            var result = _dbContext.Products.ToList();
            return View(result);
        }
        #endregion

        #region 상품상세조회

        [AllowAnonymous]
        [HttpGet("read/{id:int}")]
        public async Task<IActionResult> Read(int id)
        {
            // 상품의 Id로 해당 물건 정보 가져오기
            var result = await _service.GetByIdAsync(id);
            return View(result);
        }
        #endregion

        #region 상품수정
        [HttpGet("Edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            // 수정할 상품 정보 불러오기
            var result = _service.GetByIdAsync(id);
            return View(result.Result);
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> EditAsync(ProductModel model, IFormFileCollection files)
        {
            // 수정할 상품 정보 불러오기
            var UpdateModel = _dbContext
                            .Products
                            .Where(product => product.Id == model.Id)
                            .FirstOrDefault();
            // 파일 업로드 경로 지정
            string path = @"wwwroot/" + UpdateModel.URI;
            // 기존에 있던 이미지 삭제
            await _fileService.FileDelete(path);
            // DB에 저장되어 있는 경로 수정  
            foreach (var file in files){ UpdateModel.URI = "Seller/" + UpdateModel.RegisterUserId + "/" + file.FileName;}
            if (await _service.Upload(UpdateModel, files) > 0) { return View(UpdateModel); }

            UpdateModel.Name = model.Name;
            UpdateModel.Price = model.Price;
            _dbContext.SaveChanges();
            return Redirect("/product/list");
        }
        #endregion

        #region 상품삭제
        // 상품 삭제
        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            // 삭제할 정보 가져오기
            var result = _service.GetByIdAsync(id);
            string path = @"wwwroot/" + result.Result.URI;
            // 파일 삭제
            _fileService.FileDelete(path);
            // 상품 삭제
            await _service.DeleteAsync(result.Result.Id);            
            return Redirect("/product/list");
        }
        #endregion
        
        #region 상품 찜 하기
        // 상품 찜 하기
        [AllowAnonymous]
        [HttpGet("wish")]
        public async Task<string> Wish(int id)
        {
            var user = HttpContext.User;
            // 상품의 id와 유저의 정보를 확인해 찜하기, 정보가 있으면 취소

            var curUser = await _userManager.GetUserAsync(user);

            // 찜을 누른 상품과 좋아요를 누른 사람의 목록 가져오기
            var result = _dbContext.Products.Include(p => p.WishUsers).Where(p => p.Id == id).FirstOrDefault();

            if (result.WishUsers.Count == 0)
            {
                result.WishUsers.Add(curUser);
                _dbContext.SaveChanges();
                return "ok";
            }
            else
            {
                result.WishUsers.Remove(curUser);
                _dbContext.SaveChanges();
                return "remove";
            }
        }
        #endregion

        #region 판매자 상품 목록
        [HttpGet("myProduct")]
        public IActionResult MyProduct()
        {
            var curUser = _userManager.GetUserId(HttpContext.User);
            var result = _dbContext.Products
                .Where(p => p.RegisterUserId == curUser)
                .Include(p => p.buyListModels).ThenInclude(a => a.NewIdentityUser)
                .Include(p => p.WishUsers)
                .ToList();
            foreach(var product in result)
            {
                int a = product.buyListModels.Count();
            }
            return View(result);
            //return View("/Views/Product/MyProduct.cshtml");

        }
        #endregion
    }
}
