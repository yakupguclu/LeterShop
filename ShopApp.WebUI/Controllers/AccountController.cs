using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.WebUI.Exensions;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.MailServices;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken] //get metotları haricindeki bütün metotlar token ile validated edilmek zorunda.
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IEmailSender _emailSender;
        private ICardService _cardService;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ICardService cardService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _cardService = cardService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // generate token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });

                // send email
                await _emailSender.SendEmailAsync(model.Email, "Hesabınızı Onaylayınız.", $"Lütfen email hesabınızı onaylamak için linke <a href='http://localhost:60926{callbackUrl}'>tıklayınız.</a>");

                TempData.Put("message", new ResultMessage()
                {
                    Title = "Hesap Onayı",
                    Message = "Eposta adrenize gelen link ile hesabınızı onaylayınız",
                    Css = "warning"
                });

                return RedirectToAction("Login", "Account");
            }


            ModelState.AddModelError("", "Bilinmeyen hata oluştu lütfen tekrar deneyiniz.");
            return View(model);
        }


        /* [HttpPost]
         public async Task<IActionResult> Register(RegisterModel model)
         {
             if (!ModelState.IsValid)
             {
                 return View(model);
             }
             var user = new ApplicationUser
             {
                 UserName = model.UserName,
                 Email = model.Email,
                 FullName = model.FullName
             };

             var result = await _userManager.CreateAsync(user, model.Password);

             if (result.Succeeded)
             {
                 //genarate token
                 var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                 var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                 {
                     userId = user.Id,
                     token = code
                 });

                 //send mail
                 // await _emailSender.SendEmailAsync(model.Email,"Hesabınızı onaylayın",$"Lütfen email hesabınızı onaylamak için linke <a href='http://localhost:54885/{callbackUrl}'>tıklayınız</a> ");
                 SmtpClient smtp = new SmtpClient();
                 MailMessage mail = new MailMessage();


                 mail.IsBodyHtml = true;
                 mail.From = new MailAddress("pay@onypos.com"); //maili gönderen adres 
                 //string adsoyad = "İsim Soyism : " + collection["name"] + "<br/>";
                 //string eposta = "karamehmetzeki506@gmail.com";
                 string konu = "Hesap Onayı";
                 string mesaj = $"Lütfen email hesabınızı onaylamak için linke <a href='http://localhost:54885{callbackUrl}'>tıklayınız</a> ";
                 string Body = mesaj;
                 mail.Subject = konu;
                 mail.To.Add("ykpgclu@gmail.com"); //mail gönderilen adres 


                 mail.Body = Body;

                 smtp.Host = "smtp.gmail.com"; //mail serverının host bilgisi 
                                               // smtp.Host = "mail.onypos.com"; //mail serverının host bilgisi 
                 smtp.Port = 587; //mail serverının portu 
                 smtp.UseDefaultCredentials = true;
                 smtp.EnableSsl = true;
                 // smtp.Credentials = new System.Net.NetworkCredential("pay@onypos.com", "py.1233"); //mail serverının kullanıcı bilgileri
                 smtp.Credentials = new System.Net.NetworkCredential("infoshayazilim@gmail.com", "Laz.1453"); //mail serverının kullanıcı bilgileri

                 try
                 {
                     smtp.Send(mail);
                     ViewBag.data = "Gönderildi..!";
                     return RedirectToAction("login", "account");
                 }
                 catch (Exception)
                 {

                     ViewBag.data = "Mail Gönderilemedi..!";

                     return RedirectToAction("login", "account");
                 }

                 // return RedirectToAction("login", "account");
             }
             ModelState.AddModelError("", "Bilinmeyen bir hata oluştu lütfen tekrar deneyiniz");
             return View(model);
         }
        */
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {

            return View(new LoginModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Bu mail adresi ile daha önce hesap oluşturulmamış.");
                return View(model);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData.Put("message", new ResultMessage()
                {
                    Title = "Hesap Onayı",
                    Message = "Hesabınız henüz onaylanmadı lütfen gelen maili kontrol ediniz.",
                    Css = "warning"
                });
                ModelState.AddModelError("", "Lütfen hesabınızı mail ile onaylayınız");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
            }
            ModelState.AddModelError("", "Mail veya parola yanlış");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData["message"] = "Geçersiz token";
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //Create card object 
                    _cardService.InitializeCard(user.Id);

                    TempData.Put("message", new ResultMessage()
                    {
                        Title = "Hesap Onayı",
                        Message = "Hesabınız başarıyla onaylanmıştır.",
                        Css = "success"
                    });

                    return RedirectToAction("Login");
                }
            }
            TempData.Put("message", new ResultMessage()
            {
                Title = "Hesap Onayı",
                Message = "Hesabınız henüz onaylanmadı lütfen gelel maili kontrol ediniz.",
                Css = "warning"
            });

            //TempData["message"] = "Hesabınız onaylanmadı";
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return View();
            }

            var code = _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                token = code,
            });

            //send mail //Old

            // await _emailSender.SendEmailAsync(Email,"Parola Yenile",$"Parolanızı yenilemek için  için linke <a href='http://localhost:54885/{callbackUrl}'>tıklayınız</a> ");

            //send mail 
            // await _emailSender.SendEmailAsync(model.Email,"Hesabınızı onaylayın",$"Lütfen email hesabınızı onaylamak için linke <a href='http://localhost:54885/{callbackUrl}'>tıklayınız</a> ");
            SmtpClient smtp = new SmtpClient();
            MailMessage mail = new MailMessage();

            mail.IsBodyHtml = true;
            mail.From = new MailAddress("pay@onypos.com"); //maili gönderen adres 
                                                           //string eposta = "karamehmetzeki506@gmail.com";
            string konu = "Hesap Onayı";
            string mesaj = $"Parolanızı yenilemek için  için linke <a href='http://localhost:54885{callbackUrl}'>tıklayınız</a> ";
            string Body = mesaj;
            mail.Subject = konu;
            mail.To.Add("karamehmetzeki506@gmail.com"); //mail gönderilen adres 

            mail.Body = Body;

            smtp.Host = "smtp.gmail.com"; //mail serverının host bilgisi 
                                          // smtp.Host = "mail.onypos.com"; //mail serverının host bilgisi 
            smtp.Port = 587; //mail serverının portu 
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            // smtp.Credentials = new System.Net.NetworkCredential("pay@onypos.com", "py.1233"); //mail serverının kullanıcı bilgileri
            smtp.Credentials = new System.Net.NetworkCredential("infoshayazilim@gmail.com", "Laz.1453"); //mail serverının kullanıcı bilgileri
            try
            {
                smtp.Send(mail);
                ViewBag.data = "Gönderildi..!";
                return RedirectToAction("login", "account");
            }
            catch (Exception)
            {
                ViewBag.data = "Mail Gönderilemedi..!";
                return RedirectToAction("login", "account");
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Account", "Login");
            }

            return View(model);
        }
        public IActionResult Accesdenied()
        {
            return View();
        }

        public IActionResult MyAcount()
        {
            return View();
        }
    }
}