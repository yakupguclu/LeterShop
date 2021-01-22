using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.DataAccess.Abstract
{
   public interface ICardDal:IRepository<Card>
    {
        public void InitializeCard(string usrId);
        Card GetByUserId(string userId);
        void DeleteFromCart(int cardId, int productId);
    }
}


