using BaseProject.Data.Service;
using BaseProject.Data;
using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Controllers
{
    [Route("Inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _service;        
        private readonly UserManager<UserIdentity> _userManager;
        private readonly BaseDbContext _dbContext;
        private readonly IFileService _fileService;


        public InventoryController(
            IInventoryService service
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
        public async Task<IActionResult> CreateProduct(Inventory_Model model)
        {
            // 파일 저장
            await _service.AddAsync(model);
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
            var result =  _dbContext.Inventory_Models.Include(i => i.ProductId).ToList();
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
        // 여러개 동시에 수정할 수 있도록 바꾸기
        [HttpPost("update")]
        public async Task<IActionResult> UpdateProduct(Inventory_Model model, int count, string startTime, string endTime)
        {
            // 수정할 정보 불러오기
            // 날짜 기준으로 불러오기
            var UpdateModel = _dbContext
                            .Inventory_Models                            
                            .FirstOrDefault();

            // 수정 시간 저장
            Inventory_Edit_Log_Model log = new Inventory_Edit_Log_Model()
            {
                EditTime = DateTime.Now,
            };
            _dbContext.Inventory_Edit_Log_Model.Add(log);


            _dbContext.SaveChanges();
            return Redirect("/product/list");
        }
        #endregion
    }
}

