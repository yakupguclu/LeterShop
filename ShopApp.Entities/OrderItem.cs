﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Entities
{
   public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Price { get; set; } //Satın alınan ürünün satın alındığı fiyatını kaybetmemek için fiyat bilgisi tutulur.

        public int Quantity { get; set; }

    }
}
