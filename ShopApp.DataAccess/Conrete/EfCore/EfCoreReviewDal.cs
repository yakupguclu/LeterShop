using ShopApp.DataAccess.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.DataAccess.Conrete.EfCore
{
    public class EfCoreReviewDal : EfCoreGenericRepository<Review, ShopContext>, IReviewDal
    {
        public int Insert(Review review)
        {
            using (var context = new ShopContext())
            {
              context.Add(review);
              return   context.SaveChanges();
            }
        }
    }
}
