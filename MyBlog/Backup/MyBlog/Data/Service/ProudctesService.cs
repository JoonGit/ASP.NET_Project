using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data.Base;
using MyBlog.Models;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MyBlog.Data.Service
{
    public class ProudctesService : EntityBaseRepository<ProductModel>, IProudctesService
    {
        private readonly BlogDbContext _dbContext;
        private readonly FileService _fileService;
        public ProudctesService(BlogDbContext context, FileService fileService) : base(context)
        {
            _fileService = fileService;
        }

        private async Task<int> Upload(ProductModel model, IFormFileCollection files)
        {
            try
            {	// 여러개의 파일일 경우 하나씩 저장
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // 파일이 저장될 경로
                        string path = "wwwroot/Seller/" + model.RegisterUserId;
                        // DB에 저장되는 파일의 경로
                        model.URI = "Seller/" + model.RegisterUserId + "/" + file.FileName;
                        string fileName = Path.GetFileName(Convert.ToString(file.FileName));

                        //파일 업로드
                        _fileService.FileUpload(path, fileName, file);
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                // 파일 업로드 실패 처리
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        Task<int> IProudctesService.Upload(ProductModel model, IFormFileCollection files)
        {
            return Upload(model, files);
        }
    }
}


