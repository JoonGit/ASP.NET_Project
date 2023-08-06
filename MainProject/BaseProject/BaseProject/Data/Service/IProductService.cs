using BaseProject.Data.Base;
using BaseProject.Models;

namespace BaseProject.Data.Service
{
    public interface IProductService : IEntityBaseRepository<ProductModel>
    {
        Task<string> Upload(ProductModel model, IFormFile file);
    }
}
