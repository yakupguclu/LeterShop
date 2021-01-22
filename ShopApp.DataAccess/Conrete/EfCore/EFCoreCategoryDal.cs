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
    public class EFCoreCategoryDal : EfCoreGenericRepository<Category, ShopContext>, ICategoryDal
    {
        public void DeleteFromCategor(int categoryId, int productId)
        {
            using (var context = new ShopContext())
            {
                var cmd = $"delete from ProdctCategory where ProductId=@p0 And CategoryId=@p1";
                context.Database.ExecuteSqlRaw(cmd, productId, categoryId);
                //context.Database.ExecuteSqlCommand(cmd, productId, categoryId); //Bu yapı artık microsot tarafndan kullanılıyor 
            }
        }

        public Category GetByIdWithProducts(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Categories
                      .Where(x => x.Id == id)
                      .Include(x => x.ProdctCategories)
                      .ThenInclude(x => x.Product)
                      .FirstOrDefault();
            }
        }

        public IEnumerable<Product> GetPopularCategory()
        {
            throw new NotImplementedException();
        }
    }
}
