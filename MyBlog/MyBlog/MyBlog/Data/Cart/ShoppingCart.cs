﻿using Microsoft.EntityFrameworkCore;
using MyBlog.Models;
using System;

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
        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            // 현재 접속한 유저의 Session에 CartId가 없으면 새로 생성
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            // 현재 접속한 유저의 DB를 가져옴
            var context = services.GetService<BlogDbContext>();

            // 현재 접속한 유저의 Session에 CartId가 없으면 새로 생성
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            // 현재 접속한 유저의 Session에 CartId를 저장
            session.SetString("CartId", cartId);
            // 현재 접속한 유저의 DB에 CartId가 없으면 새로 생성
            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Include(n => n.Product).ToList());
        }

        public double GetShoppingCartTotal() => _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Select(n => n.Product.Price * n.count).Sum();
    }
}