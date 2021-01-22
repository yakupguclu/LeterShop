using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Star { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public DateTime Date { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }

    }
}

