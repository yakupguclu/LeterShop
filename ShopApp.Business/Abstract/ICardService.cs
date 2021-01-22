using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Business.Abstract
{
    public interface ICardService
    {
        void InitializeCard(string usrId);
        Card GetCardByUserId(string userId);
        void AddToCard(string userId, int productId, int quantity);
        void DeleteFromCart(string userId, int productId);
    }
}

