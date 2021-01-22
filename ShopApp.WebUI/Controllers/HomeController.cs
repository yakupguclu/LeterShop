using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Conrete.EfCore;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class HomeController : Controller
    {

        private IProductService _productService;
        public HomeController(IProductService productService)
        {
            _productService = productService;
        }
        public IActionResult Index()
        {
           // SeedDatabase.Seed();
            var productLM = new ProducListModel()
            {
                Products = _productService.GetAll().ToList()
            };

           // var users = _productService.GetAll(productLM);

            return View(productLM);
        }
    }
}