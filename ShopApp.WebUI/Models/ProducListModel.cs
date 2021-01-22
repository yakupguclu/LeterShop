using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Models
{
    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }
        public int TotalPages()
        {
            return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); //çıkan sonucu yukarı yuvarlar örn 10/3=3.3 -> ~4
        }
    }
    public class ProducListModel
    {
        public PageInfo PageInfo { get; set; }
        public List<Product> Products { get;  set; }
        public Product  Product { get;  set; }
        public List<Category> Categories { get; set; }
        public Category Category { get; set; }
        public Filter Filter { get; set; } /////////////////////////////////////

    }
}
