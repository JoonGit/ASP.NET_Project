using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBlog.Controllers;
using MyBlog.Data;
using MyBlog.Data.Cart;
using MyBlog.Data.Service;
using MyBlog.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProudctesService, ProudctesService>();
builder.Services.AddScoped<IFileService ,FileService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped(sc => OrderController.GetShoppingCart(sc));
builder.Services.AddScoped(sc => ShoppingCart.Testfun(sc));

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme
    ).AddCookie(options => {
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(60)
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/home/accessdenied";
        options.LoginPath = "/account/login";
    });

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyBlog.Data.BlogDbContext>(
    options => options.UseMySql(connection, ServerVersion.AutoDetect(connection))
    );

builder.Services.AddIdentity<NewIdentityUser, IdentityRole>(
    options => options.SignIn.RequireConfirmedAccount = false
    )
    .AddEntityFrameworkStores<BlogDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddMemoryCache();
builder.Services.AddSession();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
