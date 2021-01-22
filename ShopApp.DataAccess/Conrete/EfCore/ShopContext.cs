using Microsoft.EntityFrameworkCore;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.DataAccess.Conrete.EfCore
{
   public class ShopContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=DESKTOP-EC26JK7;Database=ShopSecDb; Trusted_Connection=True");
            optionsBuilder.UseSqlServer(@"Server=YAKUP\SQLEXPRESS;Database=ShopSecDb; Trusted_Connection=True");
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Card> Cards{ get; set; } 
        public DbSet<CardItem> CardItems{ get; set; } 
        public DbSet<Order> Orders{ get; set; } 
        public DbSet<Review> Reviews{ get; set; } 

    }
}
