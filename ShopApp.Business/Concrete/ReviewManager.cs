using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Business.Concrete
{
    public class ReviewManager : IReviewService
    {
        private IReviewDal _reviewDal;
        public ReviewManager(IReviewDal reviewDal)
        {
            _reviewDal = reviewDal;
        }
        public int Create(Review entity)
        {
            return _reviewDal.Insert(entity);
        }
    }
}
