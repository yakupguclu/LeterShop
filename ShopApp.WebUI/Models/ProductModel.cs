using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        //[Required (ErrorMessage ="Name alanı boş geçilemez")]
        //[StringLength(60,MinimumLength =10,ErrorMessage ="Lütfen 10-60 sayı arası dışında karakter yazmayınız")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "ImageUrl alanı boş geçilemez")]
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Description alanı boş geçilemez")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Lütfen 10-100 sayı arası dışında karakter yazmayınız")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Fiyat belirtiniz")]
        [Range(1,10000)]
        public decimal? Price { get; set; }
        public  List<Category> SelectedCategory { get; set; }

    }
}
