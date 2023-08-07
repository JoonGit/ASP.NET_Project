using BaseProject.Data.Base;
using BaseProject.Models;

namespace BaseProject.Data.Service
{
    public interface IProductService : IEntityBaseRepository<Product_Model>
    {
        Task<string> ImgUpload(Product_Model model, IFormFile file);
    }
}
