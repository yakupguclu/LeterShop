using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ShopApp.DataAccess.Abstract
{
   public interface ICategoryDal:IRepository<Category>
    {
        IEnumerable<Product> GetPopularCategory();
        Category GetByIdWithProducts(int id);
        void DeleteFromCategor(int categoryId, int productId);
    }
}
