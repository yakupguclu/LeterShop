using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Entities
{
    public class Order
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        // stripe API
        //Iyzico API

        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserId { get; set; } //User Identity ApNetUser tablosundaki ilişkiyi kurmak için 

        public EnumOrderState OrderState { get; set; }
        public EnumPaymentTypes PaymentTypes { get; set; }

        public string FirsName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string OrderNote { get; set; }

        //Bu alanlar ödeme bilgilerini apılere çekmek için kullanılır. Nullable olabileceği için strin olarak tanımlandı
        public  string PaymentId { get; set; }
        public string PaymentToken { get; set; }
        public string ConversationId { get; set; }  //Iyzico için kullanılır

        public List<OrderItem> OrderItems { get; set; }
    }
    public enum EnumOrderState
    {
        waiting=0,
        Unpaid=1,
        Complated=2
    }
    public enum EnumPaymentTypes
    {
        CreditCard=0,
        Eft=1,
    }
}


