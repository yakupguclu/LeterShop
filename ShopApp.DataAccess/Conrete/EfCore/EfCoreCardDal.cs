using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.DataAccess.Conrete.EfCore
{
    public class EfCoreCardDal : EfCoreGenericRepository<Card, ShopContext>, ICardDal
    {
        // EfCoreGenericRepository daki Metot'u ezme işlemi 
        public override void Update(Card entity)
        {
            using (var context = new ShopContext())
            {
                context.Cards.Update(entity);
                context.SaveChanges();
            }
        }
       
        public Card GetByUserId(string userId)
        {
          //burada tüm değerler geliyo
            using (var context = new ShopContext())
            {
                return context
                    .Cards
                    .Include(x => x.CardItems)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefault(x => x.UserId == userId);
            }
        }

        public void InitializeCard(string usrId)
        {
            throw new NotImplementedException();
        }

        public void DeleteFromCart(int cardId, int productId)
        {
            using (var context = new ShopContext())
            {
                var cmd = $"delete from CardItems where CardId=@p0 And ProductId=@p1";
                context.Database.ExecuteSqlRaw(cmd, cardId,productId);
            };
        }
    }
}

