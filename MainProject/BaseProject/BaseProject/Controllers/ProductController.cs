using BaseProject.Data;
using BaseProject.Data.Service;
using BaseProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Controllers
{
    [Route("Product")]
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
        [HttpGet("create")]
        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        // 권한을 소비자 등록자 로 나워 등록자만 접근 가능하도록 변경
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct(Product_Model model,  IFormFile ImgFile)
        {
            // 파일 저장
            model.ImgUrl = await _fileService.FileCreat(model.Name, ImgFile, "product");
            await _service.AddAsync(model);

            for (int i = 0; i < model.MetrailId.Length; i++)
            {
                Product_Use_Metrail_Model use_Metrail_Model = new Product_Use_Metrail_Model()
                {
                    ProductId = model.Id,
                    //MetrailId = model.MetrailId[i],
                    Quantity = model.count[i]
                };
                _dbContext.Product_Use_Metrail_Models.Add(use_Metrail_Model);
            }
           
            // 파일 업로드후 모델 저장
            
            await _dbContext.SaveChangesAsync();
            return Redirect("/");
        }
        #endregion

        #region 상품목록조회
        [HttpGet("Read")]
        [AllowAnonymous]
        public async Task<IActionResult> ReadProduct()
        {
            // 전체 상품 목록 조회
            // Metrail 넣고 테스트
            //var result =  _dbContext.ProductModels.Include(p => p.ProductUseMetrailModel).ToList();
            //return View(result);
            return View();
        }
        #endregion

        #region 상품수정
        [HttpGet("update")]
        public IActionResult UpdateProduct(int id)
        {
            // 수정할 상품 정보 불러오기
            var result = _service.GetByIdAsync(id);
            return View(result.Result);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateProduct(Product_Model model, IFormFile file)
        {
            // 수정할 상품 정보 불러오기
            var UpdateModel = _dbContext
                            .Product_Models
                            .Where(product => product.Id == model.Id)
                            .FirstOrDefault();
            UpdateModel.Name = model.Name;
            UpdateModel.Price = model.Price;
            // 기존에 있던 파일의 경로
            string path = @"wwwroot/" + UpdateModel.ImgUrl;
            // 기존에 있던 이미지 삭제
            await _fileService.FileDelete(path);
            // DB에 저장되어 있는 경로 수정              
            UpdateModel.ImgUrl = await _service.ImgUpload(UpdateModel, file);

            //Metrail에서도 model.Id의 해당되는 MetrailId를 가진 데이터를 삭제 후 값 추가

            // 실패시 반환된 경로
            //return View(UpdateModel);

            // 수정 시간 저장
            Product_Edit_LogModel log = new Product_Edit_LogModel()
            {
                ProductId = UpdateModel.Id,
                EditTime = DateTime.Now,
            };
            _dbContext.Product_Edit_Log_Models.Add(log);
            

            _dbContext.SaveChanges();
            return Redirect("/product/list");
        }
        #endregion

        #region 상품삭제
        // 상품 삭제
        [HttpGet("delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // 삭제할 정보 가져오기
            var Model = _dbContext
                           .Product_Models
                           .Where(product => product.Id == id)
                           .FirstOrDefault();
            Model.Status = "False";
            var result = _service.UpdateAsync(id, Model);
            return Redirect("/product/list");
        }
        #endregion
    }
}
