using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Models
{

   public class CardModel
    {
        public int CardId { get; set; }
        public List<CardItemModel> CardItems { get; set; }

        public decimal TotolPrice()
        {
            return CardItems.Sum(x => x.Price * x.Quantity);
        }
    }
    public class CardItemModel
    {
        public int CardItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
