using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Business.Concrete
{
    public class CardManager : ICardService
    {
        private ICardDal _cardDal;
        public CardManager(ICardDal cardDal)
        {
            _cardDal = cardDal;
        }
        public void AddToCard(string userId, int productId, int quantity)
        {
            var card = GetCardByUserId(userId);
            if (card!=null)
            {
                //Buradaki işlem sepetteki ürünün var olup olmadığını index numarası ile kontrol ediliyo eğer yok ise yeni ürün ekleniyor eğer var ise ürün adeti artırılıyor
                var index = card.CardItems.FindIndex(x => x.ProductId == productId); 
                if (index<0)
                {
                    card.CardItems.Add(new CardItem()
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        CardId = card.Id
                    });

                }
                else
                {
                    card.CardItems[index].Quantity += quantity;
                }
                _cardDal.Update(card);
            }
            
        }
        public void DeleteFromCart(string userId, int productId)
        {
            var card = GetCardByUserId(userId);
            if (card!=null)
            {
                _cardDal.DeleteFromCart(card.Id, productId);
            }
        }
        public Card GetCardByUserId(string userId)
        {
            //kontrol için
            var control = _cardDal.GetByUserId(userId);
            return _cardDal.GetByUserId(userId);
        }
        public void InitializeCard(string userId)
        {
            _cardDal.Create(new Card() { UserId = userId });
        }
    }
}
