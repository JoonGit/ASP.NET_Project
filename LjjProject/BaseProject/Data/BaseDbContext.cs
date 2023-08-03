using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BaseProject.Models;

namespace BaseProject.Data
{
    public class BaseDbContext : IdentityDbContext<IdentityUser>
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> option) : base(option)
        {
        }

        public DbSet<IoTDataModel> IoTDataModels { get; set; }
        
        public DbSet<IoTModel> IoTModels { get; set; }

        public DbSet<LoginLogModel> LoginLogModels { get; set; }

        public DbSet<MaterialModel> MaterialModels { get; set; }

        public DbSet<OrderModel> OrderModels { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<OrderEditLogModel> OrderEditLogModels { get; set; }        

        public DbSet<ProductEditLogModel> ProductEditLogModels { get; set; }

        public DbSet<ProductModel> ProductModels { get; set; }

        public DbSet<UserEditLogModel> UserEditLogModels { get; set;}

        

        

        








    }
}
