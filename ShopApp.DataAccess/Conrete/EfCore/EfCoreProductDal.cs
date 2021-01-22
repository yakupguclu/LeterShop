using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ShopApp.DataAccess.Conrete.EfCore
{
    public class EfCoreProductDal : EfCoreGenericRepository<Product, ShopContext>, IProductDal
    {


        public Product GetByIdWithCategories(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products
                    .Where(x => x.Id == id)
                    .Include(x => x.ProdctCategories)
                    .ThenInclude(x => x.Category)
                    .FirstOrDefault();
            }
        }

        public int GetCountByCategory(string category)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                        .Include(x => x.ProdctCategories)
                        .ThenInclude(x => x.Category)
                        .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }
                return products.Count();
            }
        }

        public IEnumerable<Product> GetPopularProducts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetPProductsByCategory(string category, int page, int pagesize)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                        .Include(x => x.ProdctCategories)
                        .ThenInclude(x => x.Category)
                        .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }
                return products.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            }
        }

        public Product GetProductDetails(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products
                    .Where(x => x.Id == id)
                    .Include(x => x.Reviews)
                    .Include(x => x.ProdctCategories)
                    .ThenInclude(x => x.Category)
                    .FirstOrDefault();
            }
        }

        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new ShopContext())
            {
                var product = context.Products
                    .Include(x => x.ProdctCategories)
                    .FirstOrDefault(x => x.Id == entity.Id);

                if (product != null)
                {
                    product.Name = entity.Name;
                    product.Description = entity.Description;
                    product.ImageUrl = entity.ImageUrl;
                    product.Price = entity.Price;

                    product.ProdctCategories = categoryIds.Select(catId => new ProdctCategory()
                    {
                        CategoryId = catId,
                        ProductId = entity.Id
                    }).ToList();
                    context.SaveChanges();
                }
            }
        }


        public List<Product> GetByIdWitFilterhCategories(Filter filter, int page, int pagesize)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.AsQueryable();
                if (filter != null)
                {
                    if (filter.Telefon != null)
                    {
                        products = products
                       .Include(x => x.ProdctCategories)
                       .ThenInclude(x => x.Category)
                       .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Telefon.ToLower()));
                    }

                    else if (filter.Elektronik != null)
                    {
                        products = products
                       .Include(x => x.ProdctCategories)
                       .ThenInclude(x => x.Category)
                       .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Elektronik.ToLower()));
                    }
                    else if (filter.Bilgisayar != null)
                    {
                        products = products
                       .Include(x => x.ProdctCategories)
                       .ThenInclude(x => x.Category)
                       .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Bilgisayar.ToLower()));
                    }

                    else if (filter.Bilgisayar != null && filter.Telefon != null)
                    {
                        products = products
                       .Include(x => x.ProdctCategories)
                       .ThenInclude(x => x.Category)
                       .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Telefon.ToLower() && a.Category.Name.ToLower() == filter.Bilgisayar.ToLower()));
                    }
                    else if (filter.Bilgisayar != null && filter.Elektronik != null)
                    {
                        products = products
                       .Include(x => x.ProdctCategories)
                       .ThenInclude(x => x.Category)
                       .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Elektronik.ToLower() && a.Category.Name.ToLower() == filter.Bilgisayar.ToLower()));
                    }
                    else if (filter.Telefon != null && filter.Elektronik != null)
                    {
                        products = products
                       .Include(x => x.ProdctCategories)
                       .ThenInclude(x => x.Category)
                       .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Telefon.ToLower() && a.Category.Name.ToLower() == filter.Elektronik.ToLower()));
                    }

                    else if (filter.Telefon != null && filter.Elektronik != null && filter.Bilgisayar != null)
                    {
                        products = products
                       .Include(x => x.ProdctCategories)
                       .ThenInclude(x => x.Category)
                       .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Telefon.ToLower() && a.Category.Name.ToLower() == filter.Elektronik.ToLower() && a.Category.Name.ToLower() == filter.Bilgisayar.ToLower()));
                    }

                }
                return products.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            }
        }

        // NOT Geçiçi bir filtreleme yöntemidir. Daha sonra filter fonksiyonu kullanılacaktır
        //public IEnumerable<Product> GetByIdWitFilterhCategories(Filter filter)
        //{
        //    if(filter != null)
        //    {
        //        using (var context = new ShopContext())
        //        {
        //            var products = context.Products.AsQueryable();
        //            if (filter.Telefon != null)
        //            {
        //                products = products
        //                .Include(x => x.ProdctCategories)
        //                .ThenInclude(x => x.Category)
        //                .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Telefon.ToLower()));
        //            }
        //            else if (filter.Bilgisayar != null)
        //            {
        //                products = products
        //                .Include(x => x.ProdctCategories)
        //                .ThenInclude(x => x.Category)
        //                .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Bilgisayar.ToLower()));
        //            }
        //           else if (filter.Elektronik != null)
        //            {
        //                products = products
        //                .Include(x => x.ProdctCategories)
        //                .ThenInclude(x => x.Category)
        //                .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Elektronik.ToLower()));
        //            }
        //            else if (filter.Elektronik != null && filter.Bilgisayar != null)
        //            {
        //                products = products
        //                .Include(x => x.ProdctCategories)
        //                .ThenInclude(x => x.Category)
        //                .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Elektronik.ToLower() && a.Category.Name.ToLower()==filter.Bilgisayar.ToLower()));
        //            }
        //            else if (filter.Elektronik != null && filter.Telefon != null)
        //            {
        //                products = products
        //                .Include(x => x.ProdctCategories)
        //                .ThenInclude(x => x.Category)
        //                .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Elektronik.ToLower() && a.Category.Name.ToLower() == filter.Telefon.ToLower()));
        //            }
        //            else if (filter.Bilgisayar != null && filter.Telefon != null)
        //            {
        //                products = products
        //                .Include(x => x.ProdctCategories)
        //                .ThenInclude(x => x.Category)
        //                .Where(x => x.ProdctCategories.Any(a => a.Category.Name.ToLower() == filter.Bilgisayar.ToLower() && a.Category.Name.ToLower() == filter.Telefon.ToLower()));
        //            }
        //            return products;
        //        }



        //    }

        //    return null;
        //}
    }
}
