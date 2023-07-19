using MyBlog.Data.Base;
using MyBlog.Models;
using System.Security.Claims;

namespace MyBlog.Data.Service
{
    public interface IProudctesService : IEntityBaseRepository<ProductModel>
    {

        Task<int> Upload(ProductModel model, IFormFileCollection files);

    }
}
