using ShopApp.DataAccess.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ShopApp.DataAccess.Conrete.Memory
{
   public class MemoryProductDal : IProductDal
    { 
        public void Create(Product entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Product entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetAll(Expression<Func<Product, bool>> filter = null)
        {
            var product = new List<Product>()
            {
                new Product(){Id=1,Name="Samsung", ImageUrl="1.jpg",Price=1000},
                new Product(){Id=2,Name="Apple",ImageUrl="2.jpg",Price=2000},
                new Product(){Id=2,Name="Xaomi",ImageUrl="3.jpg",Price=3000},

            };
            return product;
        }

        public Product GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Product> GetByIdWitFilterhCategories(Filter filter, int page, int pagesize)
        {
            throw new NotImplementedException();
        }

        public Product GetByIdWithCategories(int id)
        {
            throw new NotImplementedException();
        }

        public int GetCountByCategory(string category)
        {
            throw new NotImplementedException();
        }

        public Product GetOne(Expression<Func<Product, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetPopularProducts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetPProductsByCategory(string category, int page, int pagesize)
        {
            throw new NotImplementedException();
        }

        public Product GetProductDetails(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Product entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Product entity, int[] categoryIds)
        {
            throw new NotImplementedException();
        }
    }
}
