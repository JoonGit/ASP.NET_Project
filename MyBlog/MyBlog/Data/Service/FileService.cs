using System.IO;

namespace MyBlog.Data.Service
{
    public class FileService
    {
        public async Task FileUpload(string path, string fileName, IFormFile file)
        {
            // 경로에 파일이 없으면 생성                        
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

            // 파일 저장
            string filePath = Path.Combine(path, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
        }

        public async Task FileDelete(string path)
        {
            if (System.IO.File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (System.IO.IOException e)
                {
                    await Console.Out.WriteLineAsync(e.Message);
                    // handle exception
                }
            }
        }
        public ActivityTrackingOptions test()
        {
            return new ActivityTrackingOptions();
        }
    }
}
