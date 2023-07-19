namespace MyBlog.Data.Service
{
    public interface IFileService
    {
        Task FileUpload(string path, string fileName, IFormFile file);
        Task FileDelete(string path);
    }
}
