using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Models
{
    public class CategoryListViewModel
    {
        public string SelectedCategory { get; set; } //Seçilmiş olan kategoriyi aktif etmek için seçilmiş kategorinin bilgisini tutan yapı
        public List<Category> Categories { get; set; }
    }
}
