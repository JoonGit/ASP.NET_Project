using BaseProject.Data;
using BaseProject.Data.Service;
using BaseProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _service;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly BaseDbContext _dbContext;
        private readonly IFileService _fileService;


        public ProductController(
            IProductService service
            , UserManager<UserIdentity> userManager
            , BaseDbContext dbContext
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
            ViewBag.userId = _userManager.GetUserId(HttpContext.User);
            return View();
        }

        // 권한을 소비자 등록자 로 나워 등록자만 접근 가능하도록 변경
        [HttpPost("register")]
        public async Task<IActionResult> Register(ProductModel model, IFormFile file)
        {
            // 파일 저장
            await _service.Upload(model, file);
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
            var result =  _dbContext.ProductModels.ToList();
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
        public async Task<IActionResult> EditAsync(ProductModel model, IFormFile file)
        {
            // 수정할 상품 정보 불러오기
            var UpdateModel = _dbContext
                            .ProductModels
                            .Where(product => product.Id == model.Id)
                            .FirstOrDefault();
            UpdateModel.Name = model.Name;
            UpdateModel.Price = model.Price;
            // 기존에 있던 파일의 경로
            string path = @"wwwroot/" + UpdateModel.ImgUrl;
            // 기존에 있던 이미지 삭제
            await _fileService.FileDelete(path);
            // DB에 저장되어 있는 경로 수정              
                UpdateModel.ImgUrl = await _service.Upload(UpdateModel, file);


            // 실패시 반환된 경로
            //return View(UpdateModel);


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
            var Model = _dbContext
                           .ProductModels
                           .Where(product => product.Id == id)
                           .FirstOrDefault();
            Model.Status = "false";
            var result = _service.UpdateAsync(id, Model);
            return Redirect("/product/list");
        }
        #endregion
    }
}
