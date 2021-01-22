using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entities;
using ShopApp.WebUI.Exensions;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public IActionResult ProductList()
        {
            ProducListModel producListModel = new ProducListModel();
            producListModel.Products = _productService.GetAll();
            return View(producListModel);
        }

        public IActionResult CreateProduct()
        {
           //var product= _productService.GetById(1);

           // var model = new ProductModel()
           // {
           //     Name = product.Name,
           //     Description=product.Description,
           //     Price=product.Price,
           //     ImageUrl=product.ImageUrl
           // };

            return View(new ProductModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var entity = new Product()
                    {
                        Name = model.Name,
                        Price = model.Price,
                        Description = model.Description,
                       // ImageUrl = model.ImageUrl
                       ImageUrl = file.FileName
                     };

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    if (_productService.Create(entity))
                    {
                        return RedirectToAction("ProductList");
                    }
                }
             
                ViewBag.ErrorMessage = _productService.ErrorMessage;
                return View(model);
            }
            return View(model);
            
        }

        [HttpGet]
        public IActionResult EditProduct(int? id)
        {

            var product = _productService.GetByIdWithCategories((int)id);
            var model = new ProductModel()
            {
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Id=product.Id,
                SelectedCategory=product.ProdctCategories.Select(x=>x.Category).ToList()
            };
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model,int[] categoryIds, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = _productService.GetById(model.Id);

                if (entity == null)
                {
                    return NotFound();
                }

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Price = model.Price;
                //entity.ImageUrl = model.ImageUrl;

                if (file != null)
                {
                    entity.ImageUrl = file.FileName;

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", file.FileName);
                    using(var stream=new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                _productService.Update(entity, categoryIds);
                return RedirectToAction("ProductList");
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
           
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var entity = _productService.GetById(id);
            if(entity!=null)
            {
                _productService.Delete(entity);
            }

            //Bu alanı artık tüm işlem sonuçları içn kullanabiliriz
            TempData.Put("message", new ResultMessage()
            {
                Title="Silme başarılı",
                Message="Silme işlemi başarılı bir şekilde gerçekleşti",
                Css = "warning"
            });
            return RedirectToAction("ProductList");
        }

        public IActionResult CategoryList()
        {
            return View(new CategoryListModel()
            {
                Categories = _categoryService.GetAll()
            });
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model) 
        {
            var entity = new Category()
            {
                Name = model.Name
            };
            _categoryService.Create(entity);

            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var entity = _categoryService.GetByIdWithProducts(id);
            return View(new CategoryModel()
            {
                CategoryId = entity.Id,
                Name = entity.Name,
                Products = entity.ProdctCategories.Select(p => p.Product).ToList()
            });
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            var entity = _categoryService.GetById(model.CategoryId);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;
            _categoryService.Update(entity);

            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var entity = _categoryService.GetById(id);
            if (entity != null)
            {
                _categoryService.Delete(entity);
            }
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteFromCategory(int categoryId, int productId)
        {
            _categoryService.DeleteFromCategor(categoryId, productId);
            return Redirect("/Admin/EditCategory/"+categoryId);
        }
    }
}

