using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProdctCategory>  ProdctCategories { get; set; }
    }
}
