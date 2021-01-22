using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entities;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class ShopController : Controller
    {
        private IProductService _productService;
       // private ICategoryService _categoryService;
        public ShopController(
            IProductService productService)
        {
            _productService = productService;
           // _categoryService = categoryService;
        }
        public IActionResult Details(int? id)
        {
            if(id==null)
                {
                    return NotFound();
                }

            Product product = _productService.GetProductDetails((int)id);
                //Categories = _categoryService.GetAll()
                
            if (product == null)
            {
                return NotFound();
            }
            return View(new ProductDetailsModel()
            {
                Product = product,
                Categories = product.ProdctCategories.Select(x => x.Category).ToList(),
                Reviews = product.Reviews
            }) ;
        }

        //products/telefon?page=1 örnektir.
        public IActionResult List(string category, int page=1,Filter filter=null)
        {
            const int pagesize = 3;
           

            var productLM = new ProducListModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pagesize,
                    CurrentCategory=category
                },
                Products = _productService.GetByIdWitFilterhCategories(filter,page,pagesize).ToList()
            };
            return View(productLM);    


        }
    }
}