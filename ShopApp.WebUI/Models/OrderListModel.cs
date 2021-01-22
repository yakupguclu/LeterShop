﻿using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Models
{
    public class OrderListModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserId { get; set; }

        public EnumOrderState OrderState { get; set; }
        public EnumPaymentTypes PaymentTypes { get; set; }

        public string FirsName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string OrderNote { get; set; }
     
        public List<OrderItemModel> OrderItems { get; set; }

       

        public decimal TotolPrice()
        {
            decimal total= OrderItems.Sum(x => x.Price * x.Quantity);
            return OrderItems.Sum(x => x.Price * x.Quantity);
        }
    }


    public class OrderItemModel
    {
    
        public int OrderItemId { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Quantity { get; set; }
        public int ProductId { get; set; }
        public Review Review { get; set; }

    }
}


