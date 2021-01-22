using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.ViewComponents
{
    public class CategoryListViewComponent:ViewComponent
    {
       private ICategoryService _categoryService;
        public CategoryListViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public IViewComponentResult Invoke()
        {
            return View(new CategoryListViewModel()
            {
                SelectedCategory=RouteData.Values["category?"]?.ToString().ToLower(), //url den gelen kategori kısmındaki ismi alır. Örn Products/tefon SelectedCategory=telefon ? nul olup olmadıını kontrol eder
                Categories = _categoryService.GetAll()
            });
        } 
    }
}
