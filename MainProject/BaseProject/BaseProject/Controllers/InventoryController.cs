﻿using BaseProject.Data.Service;
using BaseProject.Data;
using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BaseProject.Data.Static;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BaseProject.Controllers
{
    //[Authorize(Roles = UserRoles.InventoryManager)]
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
        public async Task<IActionResult> CreateInventory()
        {
            var result = await _dbContext
                .Product_Models
                .ToListAsync();
            return View(result);
        }
        // 권한을 소비자 등록자 로 나워 등록자만 접근 가능하도록 변경
        [HttpPost("create")]
        public async Task<IActionResult> CreateInventory(Inventoy_Get_Info_Model model)
        {
            // 파일 저장
            for (int i = 0; i < model.ProductId.Length; i++)
            {
                if (model.Count[i] != 0)
                {
                    var product = await _dbContext.Product_Models.FindAsync(model.ProductId[i]);
                    product.Quantity += model.Count[i];
                    Inventory_Model inventory_model = new Inventory_Model()
                    {
                        ProductId = model.ProductId[i],
                        Count = model.Count[i],
                        CreateTime = model.CreateTime,
                    };
                    _dbContext.Inventory_Models.Add(inventory_model);
                }                
            }
            await _dbContext.SaveChangesAsync();
            return Redirect("/Inventory/Read");
        }
        #endregion

        #region 상품목록조회
        
        [HttpGet("Read")]
        [AllowAnonymous]
        public async Task<IActionResult> ReadInventory()
        {
            //var result = await _dbContext.Inventory_Models
            //        .Include(p => p.Product)
            //        .ToListAsync();      
            ViewBag.ProductsName = await Filter();
            return View();
        }
        [HttpPost("Read")]
        public async Task<IActionResult> ReadInventory(string name)
        {
            var result = new List<Inventory_Model>();
            if(name != "전체")
            {
                result = await _dbContext.Inventory_Models
                        .Include(p => p.Product)
                        .Where(p => p.Product.Name == name)
                        .ToListAsync();
            }
            else if( name == "전체")
            {
                  result = await _dbContext.Inventory_Models
                        .Include(p => p.Product)
                        .ToListAsync();
            }
            
            ViewBag.ProductsName = await Filter();
            return View(result);
        }

        public async Task<List<SelectListItem>> Filter()
        {
            var result = await _dbContext.Product_Models.Select(p => new SelectListItem()
            {
                Value = p.Name,
                Text = p.Name
            })
                .ToListAsync();
            result.Add(new SelectListItem() { Value = "전체", Text = "전체" });
            return result;
        }
        #endregion
        
        [HttpGet("detail")]
        public IActionResult DetailInventory(int id)
        {
            // 상품 상세 정보 조회
            var result = _dbContext.Inventory_Models
                .Include(p => p.Product)
                .Where(p => p.Id == id)
                .First();
            return View(result);
        }
        
        #region 상품수정
        [HttpGet("update")]
        public IActionResult UpdateInventory(int id)
        {
            // 수정할 상품 정보 불러오기
            var result = _service.GetByIdAsync(id);
            return View(result.Result);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateInventory(Inventory_Model model, IFormFile file)
        {
            // 수정할 정보 불러오기
            var UpdateModel = await _dbContext.Inventory_Models
                .Where(i => i.Id == model.Id)
                .FirstAsync();

            UpdateModel.Count = model.Count;
            UpdateModel.CreateTime = model.CreateTime;

            // 수정 시간 저장
            Inventory_Edit_Log_Model log = new Inventory_Edit_Log_Model()
            {
                InventoryId = UpdateModel.Id,
                EditTime = DateTime.Now,
            };
            _dbContext.Inventory_Edit_Log_Model.Add(log);
            _dbContext.SaveChanges();
            return Redirect("/Inventory/Read");
        }
        #endregion
    }
}

