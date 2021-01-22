using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IyzipayCore;
using IyzipayCore.Model;
using IyzipayCore.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entities;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        private ICardService _cardService;
        private UserManager<ApplicationUser> _userManager;
        private IOrderService _orderService;
        private IReviewService _reviewService;
        private IProductService _productService;

        public CardController(
            ICardService cardService,
            UserManager<ApplicationUser> userManager, 
            IOrderService orderService,
            IReviewService reviewService, IProductService productService)
        {
            _cardService = cardService;
            _userManager = userManager;
            _orderService = orderService;
            _reviewService = reviewService;
            _productService = productService;
        }
        public IActionResult Index()
        {

            var cart = _cardService.GetCardByUserId(_userManager.GetUserId(User));

            return View(new CardModel()
            {
                CardId = cart.Id,
                CardItems = cart.CardItems.Select(i => new CardItemModel()
                {
                    CardItemId = i.Id,
                    ProductId = i.Product.Id,
                    Name = i.Product.Name,
                    Price = (decimal)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        [HttpPost]
        public IActionResult AddToCard(int productId, int quantity)
        {
            _cardService.AddToCard(_userManager.GetUserId(User), productId, quantity);
            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {

            _cardService.DeleteFromCart(_userManager.GetUserId(User), productId);

            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = _cardService.GetCardByUserId(_userManager.GetUserId(User));
            var orderModel = new OrderModel();

            orderModel.CardModel = new CardModel()
            {
                CardId = cart.Id,
                CardItems = cart.CardItems.Select(i => new CardItemModel()
                {
                    CardItemId = i.Id,
                    ProductId = i.Product.Id,
                    Name = i.Product.Name,
                    Price = (decimal)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity
                }).ToList()
            };
            return View(orderModel);
        }

        [HttpPost]
        public IActionResult Checkout(OrderModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var card = _cardService.GetCardByUserId(userId);
                
                model.CardModel = new CardModel()
                {
                    CardId = card.Id,
                    CardItems = card.CardItems.Select(i => new CardItemModel()
                    {
                        CardItemId = i.Id,
                        ProductId = i.Product.Id,
                        Name = i.Product.Name,
                        Price = (decimal)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity = i.Quantity
                    }).ToList()
                };

                //Ödeme
                var payment = PaymentProcess(model);

                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);
                    ClearCart(model.CardModel.CardItems);
                    return View("Success");
                }
                //Sipariş
            }

            return View(model);
        }

        private void ClearCart(List<CardItemModel> cardItems)
        {
            foreach (var item in cardItems)
            {
                _cardService.DeleteFromCart(_userManager.GetUserId(User), item.ProductId);
            }
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            var order = new Order();

            order.OrderNumber = new Random().Next(111111, 999999).ToString();
            order.OrderState = EnumOrderState.Complated;
            order.PaymentTypes = EnumPaymentTypes.CreditCard;
            order.PaymentId = payment.PaymentId;
            order.ConversationId = payment.ConversationId;
            order.OrderDate = new DateTime();
            order.FirsName = model.FirsName;
            order.LastName = model.LastName;
            order.Email = model.Email;
            order.Phone = model.Phone;
            order.Adress = model.Adress;
            order.UserId = userId;

            foreach (var item in model.CardModel.CardItems)
            {
                var orderitem = new OrderItem()
                {
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems.Add(orderitem);
            }

            _orderService.Create(order);
        }

        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-8fHUIOWiWLhvysuCiMhclEobkOsZHS9H";
            options.SecretKey = "sandbox-nAZl4hck9hn4mEA9oTAog5bfGIFcSxZi";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = Guid.NewGuid().ToString(); //Guid benzersiz bir değer üretir
            request.Price = model.CardModel.TotolPrice().ToString().Split(",")[0]; ;
            request.PaidPrice = model.CardModel.TotolPrice().ToString().Split(",")[0]; ;
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = model.CardModel.CardId.ToString();
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber =model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;//.ToString()
            paymentCard.ExpireYear = model.ExpirationYear;//.ToString()
            paymentCard.Cvc = model.Cvv;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            //paymentCard.CardHolderName = "John Doe";
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";
            //paymentCard.RegisterCard = 0;

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = "John";
            buyer.Surname = "Doe";
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            foreach (var item in model.CardModel.CardItems)
            {
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Name;
                basketItem.Category1 = "Phone";
                basketItem.Category2 = "Accessories";
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItem.Price = item.Price.ToString().Split(",")[0]; //split metodu virgülle ayırma işlemi yapar
                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            return Payment.Create(request, options);


           
        }

        public IActionResult GetOrders()
        {
            var orders = _orderService.GetOrders(_userManager.GetUserId(User));
            var orderListMOdel = new List<OrderListModel>();
            OrderListModel orderModel;

            foreach (var order in orders)
            {
                orderModel = new OrderListModel();
                orderModel.OrderId = order.Id;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OrderDate = order.OrderDate;
                orderModel.OrderNote = order.OrderNote;
                orderModel.Phone = order.Phone;
                orderModel.FirsName = order.FirsName;
                orderModel.LastName = order.LastName;
                orderModel.Email = order.Email;
                orderModel.Adress = order.Adress;
                orderModel.City = order.City;

                orderModel.OrderItems = order.OrderItems.Select(x => new OrderItemModel()
                {
                    ProductId = x.ProductId,
                    OrderItemId = x.Id,
                    Name = x.Product.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ImageUrl = x.Product.ImageUrl

                }).ToList();

                orderListMOdel.Add(orderModel);
            }


            return View(orderListMOdel);
        }


        [HttpPost]
        public async Task<IActionResult> AddReview(Review review, int? productid,int yildiz=1)
        {
            var user = await _userManager.GetUserAsync(User);
            review.UserId = _userManager.GetUserId(User);
            review.UserFullName = user.FullName;
            review.Date = DateTime.Now;
            review.Star = yildiz;

            if (_reviewService.Create(review) > 0)
            {
                return Json(new { result = true }, new Newtonsoft.Json.JsonSerializerSettings());
            }

            return Json(new { result = false }, new Newtonsoft.Json.JsonSerializerSettings());


        }
    }
}

