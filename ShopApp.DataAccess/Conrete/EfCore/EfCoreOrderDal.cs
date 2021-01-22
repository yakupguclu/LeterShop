using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopApp.DataAccess.Conrete.EfCore
{
    public class EfCoreOrderDal : EfCoreGenericRepository<Order, ShopContext>, IOrderDal
    {
        //83. video 
        public List<Order> GetOrders(string userId)
        {

            using(var context =new ShopContext())
            {
                var orders = context.Orders
                    .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Product)
                    .AsQueryable();  //burda userId yi kontrol etmek için sorguyu bekletiyoruz

                //usr id boş değil ise
                if (!string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(x => x.UserId == userId);
                }
                return orders.ToList(); //sorguyu database gönder
            }
            throw new NotImplementedException();
        }
    }
}
