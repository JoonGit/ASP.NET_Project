using BaseProject.Data.Service;
using BaseProject.Data;
using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BaseProject.Controllers
{
    [Route("materal")]
    public class MateralController : Controller
    {
        
            private readonly IMateralService _service;
            private readonly UserManager<UserIdentity> _userManager;
            private readonly BaseDbContext _dbContext;
            private readonly IFileService _fileService;

            public MateralController(
                IMateralService service
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
            public async Task<IActionResult> CreateMateral()
            {
                return View();
            }

            // 권한을 수준으로 실행 가능
            [HttpPost("create")]
            public async Task<IActionResult> CreateMateral(Material_Model model, IFormFile ImgFile)
            {
                // 파일 저장
                model.ImgUrl = await _fileService.FileCreat(model.Name, ImgFile, "Materal");
                model.Status = "true";
                await _service.AddAsync(model);

                await _dbContext.SaveChangesAsync();
                return Redirect("/");
            }
            #endregion

            #region 상품목록조회
            [HttpGet("Read")]
            [AllowAnonymous]
            public async Task<IActionResult> ReadMateral()
            {
            // 전체 상품 목록 조회
            // Metrail 넣고 테스트
            var result = _dbContext.Material_Models.ToList();
            //return View(result);
            return View(result);
            }
            #endregion

            #region 상품수정
            [HttpGet("update")]
            public IActionResult UpdateMateral(int id)
            {
                // 수정할 상품 정보 불러오기
                var result = _service.GetByIdAsync(id);
                return View(result.Result);
            }
            [HttpPost("update")]
            public async Task<IActionResult> UpdateMateral(Material_Model model, IFormFile file)
            {
                // 수정할 상품 정보 불러오기
                var UpdateModel = _dbContext
                                .Material_Models
                                .Where(m => m.Id == model.Id)
                                .FirstOrDefault();
                UpdateModel.Name = model.Name;
                UpdateModel.Quantity = model.Quantity;
                UpdateModel.Price = model.Price;
                UpdateModel.Status = model.Status;
                // 기존에 있던 파일의 경로
                string path = @"wwwroot/" + UpdateModel.ImgUrl;
         
            if(file != null)
            {
                UpdateModel.ImgUrl = await _fileService.FileUpdate(UpdateModel.Name, file, "product");
            }
                

                //Metrail에서도 model.Id의 해당되는 MetrailId를 가진 데이터를 삭제 후 값 추가

                // 실패시 반환된 경로
                //return View(UpdateModel);

                //// 수정 시간 저장
                //Material_Edit_Log_Model log = new Material_Edit_Log_Model()
                //{
                //    ProductId = UpdateModel.Id,
                //    EditTime = DateTime.Now,
                //};
                //_dbContext.Product_Edit_Log_Models.Add(log);


                _dbContext.SaveChanges();
                return Redirect("/product/list");
            }
            #endregion

            #region 상품삭제
            // 상품 삭제
            [HttpGet("delete")]
            public async Task<IActionResult> DeleteMateral(int id)
            {
                // 삭제할 정보 가져오기
                var Model = _dbContext
                               .Material_Models
                               .Where(m => m.Id == id)
                               .FirstOrDefault();
                Model.Status = "False";
                var result = _service.UpdateAsync(id, Model);
                return Redirect("/product/list");
            }
            #endregion
        }
    }
