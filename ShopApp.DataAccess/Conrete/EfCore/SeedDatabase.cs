using Microsoft.EntityFrameworkCore;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.DataAccess.Conrete.EfCore
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            var context = new ShopContext();

            if (context.Database.GetPendingMigrations().Count() == 0)//eğer bekleyen bir migration yoksa . 
            {
                if (context.Categories.Count() == 0)
                {
                    context.Categories.AddRange(Categories);
                }
                if (context.Products.Count() == 0)
                {
                    context.Products.AddRange(Products);
                    context.AddRange(productCategory);
                }
                context.SaveChanges();
            }
        }
        private static Category[] Categories =
        {
            new Category(){Name="Telefon"},
            new Category(){Name="Bilgisayar"},
            new Category(){Name="Elektronik"}
        };

        private static Product[] Products =
       {
            new Product(){Name="Samsung S5",Price=2000,ImageUrl="1.jpg",Description="<p>Güzel Telefon<p>"},
            new Product(){Name="Samsung S6",Price=3000,ImageUrl="2.jpg",Description="<p>Güzel Telefon<p>"},
            new Product(){Name="Samsung S7",Price=4000,ImageUrl="3.jpg",Description="<p>Güzel Telefon<p>"},
            new Product(){Name="Samsung S8",Price=5000,ImageUrl="4.jpg",Description="<p>Güzel Telefon<p>"},
            new Product(){Name="Samsung S9",Price=6000,ImageUrl="5.jpg",Description="<p>Güzel Telefon<p>"},
            new Product(){Name="Iphone 6",Price=7000,ImageUrl="6.jpg",Description="<p>Güzel Telefon<p>"},
            new Product(){Name="Iphone 7",Price=8000,ImageUrl="7.jpg",Description="<p>Güzel Telefon<p>"},
            
        };

        private static ProdctCategory[] productCategory =
        {
            new ProdctCategory()  {Product=Products[0],Category=Categories[0]},
            new ProdctCategory()  {Product=Products[0],Category=Categories[0]},
            new ProdctCategory()  {Product=Products[1],Category=Categories[2]},
            new ProdctCategory()  {Product=Products[2],Category=Categories[1]},
            new ProdctCategory()  {Product=Products[3],Category=Categories[0]},
            new ProdctCategory()  {Product=Products[0],Category=Categories[2]},
            new ProdctCategory()  {Product=Products[2],Category=Categories[0]},
            new ProdctCategory()  {Product=Products[3],Category=Categories[1]}
        };
    }


}


