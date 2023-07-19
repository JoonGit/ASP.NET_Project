
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBlog.Models;

namespace MyBlog.Data
{
    public class BlogDbContext : IdentityDbContext<IdentityUser>
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> option) : base(option)
        {
        }

        public DbSet<ProductModel> Products { get; set; }
        public DbSet<BuyListModel> BuyLists { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<OrderModel> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.RegisterUser)
                .WithMany(u => u.RegisterModels);

            modelBuilder.Entity<ProductModel>()
                .HasMany(p => p.WishUsers)
                .WithMany(u => u.WishProducts)
                .UsingEntity("wish_user_product");

            modelBuilder
            .Entity<BuyListModel>()
            .HasKey(b => b.Id);


            modelBuilder
            .Entity<BuyListModel>()
            .HasOne(b => b.ProductModel)
            .WithMany(p => p.buyListModels);

            modelBuilder
                .Entity<BuyListModel>()
                .HasOne(l => l.NewIdentityUser)
                .WithMany(l => l.buyListModels);

            //modelBuilder.Entity<PostModel>()
            //    .HasMany(p => p.LikeUsers)
            //    .WithMany(u => u.LikePosts)
            //    .UsingEntity("like_post_user");
        }

        public DbSet<MyBlog.Models.LoginModel>? LoginModel { get; set; }
    }
}

