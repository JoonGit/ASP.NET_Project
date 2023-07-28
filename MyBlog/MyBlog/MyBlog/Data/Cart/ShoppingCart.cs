using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MyBlog.Models;
using System;
using System.Security.Claims;

namespace MyBlog.Data.Cart
{
    public class ShoppingCart
    {
        public BlogDbContext _context { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public string ShoppingCartId { get; set; }
        public ShoppingCart(BlogDbContext context)
        {
            _context = context;
        }
        public static ShoppingCart Testfun(IServiceProvider services)
        {
            var context = services.GetService<BlogDbContext>();
            string cartId = "test";
            Console.WriteLine("testfun");
            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }
        
        public void AddItemToCart(ProductModel product, int count)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.Product.Id == product.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = ShoppingCartId,
                    Product = product,
                    Count = count
                };
                _context.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Count = count;
            }
            _context.SaveChanges();
        }

        public void RemoveItemFromCart(ProductModel product)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.Product.Id == product.Id && n.ShoppingCartId == ShoppingCartId);

            _context.ShoppingCartItems.Remove(shoppingCartItem);
            _context.SaveChanges();
        }

        public void EditItemFromCart(ProductModel product, int count)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.Product.Id == product.Id && n.ShoppingCartId == ShoppingCartId);

            shoppingCartItem.Count = count;

            //if (shoppingCartItem != null)
            //{
            //    if (shoppingCartItem.Count > 1)
            //    {
            //        shoppingCartItem.Count = count;
            //    }
            //    else
            //    {
            //        _context.ShoppingCartItems.Remove(shoppingCartItem);
            //    }
            //}
            _context.SaveChanges();
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Include(n => n.Product).ToList());
        }

        public double GetShoppingCartTotal() => _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Select(n => n.Product.Price * n.Count).Sum();

        public async Task ClearShoppingCartAsync()
        {
            var items = await _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToListAsync();
            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }



    }
}
