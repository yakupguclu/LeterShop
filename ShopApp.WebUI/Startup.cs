using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Abstract;
using ShopApp.DataAccess.Conrete.EfCore;
using ShopApp.DataAccess.Conrete.Memory;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.MailServices;
using ShopApp.WebUI.Middlewares;

namespace ShopApp.WebUI
{

    public class Startup
    {

        //Configuration daki inject iþlemi appsettings.json kýlasýndaki bilgilere eriþmek için yazýlýr
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Identity sýnýflarýmýz buradaki  IdentityConnection metodu ile database ekleniyor.
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));
           // services.AddDbContext<ShopContext>();
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                //password
                options.Password.RequireDigit = true;//mutlaka numerik karakter girilmeli
                options.Password.RequireLowercase = true;//mutlaka küçük harf girilmeli;
                options.Password.RequireUppercase = true;//mutlaka büyük harf girilmeli;
                options.Password.RequiredLength = 6; //mutlaka 6 karakter olmalý
                options.Password.RequireNonAlphanumeric = false; //alfa numerik girmek zorunda olmaz

                options.Lockout.MaxFailedAccessAttempts = 5; // en fazla beþ kere yanlýþ þifre deneyebilir.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // 5 dakika sonra tekrar þifre deneyebilir

                // options.User.AllowedUserNameCharacters = ""; // kullanýcý adý içerisinde alýnmasý istenmeyen karkterler  örnek "ý,þ,ç,ð,ö".
                options.User.RequireUniqueEmail = true; // ayný mail ile bir daha  hesap oluþturmaz

                options.SignIn.RequireConfirmedEmail = true; //mail ile doðrulama gerçekleþtirir.
                options.SignIn.RequireConfirmedPhoneNumber = false;//telefon doðrulamasý kapalý
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login"; //login sayfasýnýn yolu
                options.LogoutPath = "/account/logout";//çýkþ sayfasý
                options.AccessDeniedPath = "/account/accesdenied";//yetkisi olmayan bir yere girdiðinde yönlendirilecek sayfa.
                options.SlidingExpiration = true; //kullanýcýnýn cookie süresi belli sürede biter ve giriþ ister
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //Cookie nin saklanma süresi 60dk. Not: Varsayýlaný 20dk 
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true, //scripler okunamaz güvenlik açýsýndan önemli
                    Name = "ShopApp.Securty.Cookie", //Cookie nin özel ismi
                    SameSite = SameSiteMode.Strict //Baþka bir kullanýcý bizim cookie yi alýp server a gönderemez. Csrf 
                };
            });

            //hem businesteki hem de dataacces dün alýnan hata unutma
            //Unable to resolve service for type 'ShopApp.Business.Abstract.IReviewService' while attempting to activate 'ShopApp.WebUI.Controllers.CardController'.tarzýnda  hata verir

            // services.AddScoped<IProductDal, MemoryProductDal>();
            services.AddScoped<IProductDal, EfCoreProductDal>();
            services.AddScoped<ICategoryDal, EFCoreCategoryDal>();
            services.AddScoped<ICardDal, EfCoreCardDal>();
            services.AddScoped<IOrderDal, EfCoreOrderDal>();
            services.AddScoped<IReviewDal, EfCoreReviewDal>();

            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICardService, CardManager>();
            services.AddScoped<IOrderService, OrderManager>();
            services.AddScoped<IReviewService, ReviewManager>();



            //e-mail
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMvc(options => options.EnableEndpointRouting = false);

        }


        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SeedDatabase.Seed();
            }
            //app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();
            app.CustomStaticFiles();
            //app.UseRouting();
            //app.UseCors();
            app.UseAuthentication();//Identity iþlemi içindir
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "adminProducts",
                    template: "admin/products",
                    defaults: new { controller = "Admin", action = "ProductList" }
                );
                routes.MapRoute(
                   name: "adminProducts1",
                   template: "admin/products/{id?}",
                   defaults: new { controller = "Admin", action = "EditProduct" }
               );

                routes.MapRoute(
                    name: "products",
                    template: "products/{category?}",
                    defaults: new { controller = "Shop", action = "List" }
                );

                routes.MapRoute(
                  name: "card",
                  template: "card",
                  defaults: new { controller = "Card", action = "Index" }
              );
                routes.MapRoute(
                 name: "checkout",
                 template: "checkout",
                 defaults: new { controller = "Card", action = "checkout" }
             );
                routes.MapRoute(
                 name: "orders",
                 template: "orders",
                 defaults: new { controller = "Card", action = "GetOrders" }
             );
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });

                //app.UseEndpoints(endpoints =>
                //{
                //    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
                //});
            });

           SeedIdentity.Seed(userManager, roleManager, Configuration).Wait();
        }
    }


    //public class Startup
    //{

    //    //Configuration daki inject iþlemi appsettings.json kýlasýndaki bilgilere eriþmek için yazýlýr
    //   public IConfiguration   Configuration { get; set; }
    //    public Startup(IConfiguration configuration)
    //    {
    //        Configuration = configuration;
    //    }

    //    // This method gets called by the runtime. Use this method to add services to the container.
    //    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    //    public void ConfigureServices(IServiceCollection services)  
    //    {
    //        //Identity sýnýflarýmýz buradaki  IdentityConnection metodu ile database ekleniyor.
    //        services.AddDbContext<ApplicationIdentityDbContext>(options =>
    //        options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));
    //        services.AddIdentity<ApplicationUser, IdentityRole>()
    //            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    //            .AddDefaultTokenProviders();
    //        services.Configure<IdentityOptions>(options =>
    //        {
    //            //password
    //            options.Password.RequireDigit = true;//mutlaka numerik karakter girilmeli
    //            options.Password.RequireLowercase = true;//mutlaka küçük harf girilmeli;
    //            options.Password.RequireUppercase = true;//mutlaka büyük harf girilmeli;
    //            options.Password.RequiredLength = 6; //mutlaka 6 karakter olmalý
    //            options.Password.RequireNonAlphanumeric = false; //alfa numerik girmek zorunda olmaz

    //            options.Lockout.MaxFailedAccessAttempts = 5; // en fazla beþ kere yanlýþ þifre deneyebilir.
    //            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // 5 dakika sonra tekrar þifre deneyebilir

    //            // options.User.AllowedUserNameCharacters = ""; // kullanýcý adý içerisinde alýnmasý istenmeyen karkterler  örnek "ý,þ,ç,ð,ö".
    //            options.User.RequireUniqueEmail = true; // ayný mail ile bir daha  hesap oluþturmaz

    //            options.SignIn.RequireConfirmedEmail = true; //mail ile doðrulama gerçekleþtirir.
    //            options.SignIn.RequireConfirmedPhoneNumber = false;//telefon doðrulamasý kapalý
    //        });

    //        services.ConfigureApplicationCookie(options =>
    //        {
    //            options.LoginPath = "/account/login"; //login sayfasýnýn yolu
    //            options.LogoutPath = "/account/logout";//çýkþ sayfasý
    //            options.AccessDeniedPath = "/account/accesdenied";//yetkisi olmayan bir yere girdiðinde yönlendirilecek sayfa.
    //            options.SlidingExpiration = true; //kullanýcýnýn cookie süresi belli sürede biter ve giriþ ister
    //            options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //Cookie nin saklanma süresi 60dk. Not: Varsayýlaný 20dk 
    //            options.Cookie = new CookieBuilder
    //            {
    //                HttpOnly = true, //scripler okunamaz güvenlik açýsýndan önemli
    //                Name = "ShopApp.Securty.Cookie", //Cookie nin özel ismi
    //                SameSite=SameSiteMode.Strict //Baþka bir kullanýcý bizim cookie yi alýp server a gönderemez. Csrf 
    //            };
    //        });

    //        //Not Burda hem businesteki hem de dataacces teki sýnýflarý tanýmlamalýyoz yoksa  Unable to resolve service for type 'ShopApp.Business.Abstract.IReviewService' while attempting to activate 'ShopApp.WebUI.Controllers.CardController'.tarzýnda  hata verir

    //        // services.AddScoped<IProductDal, MemoryProductDal>();
    //        services.AddScoped<IProductDal, EfCoreProductDal>();
    //        services.AddScoped<ICategoryDal, EFCoreCategoryDal>();
    //        services.AddScoped<ICardDal, EfCoreCardDal>();
    //        services.AddScoped<IOrderDal, EfCoreOrderDal>();
    //        services.AddScoped<IReviewDal, EfCoreReviewDal>();

    //        services.AddScoped<IProductService, ProductManager>();
    //        services.AddScoped<ICategoryService, CategoryManager>();
    //        services.AddScoped<ICardService, CardManager>();
    //        services.AddScoped<IOrderService, OrderManager>();
    //        services.AddScoped<IReviewService,ReviewManager >();

    //        //sen e mail services
    //        services.AddTransient<IEmailSender,EmailSender>();
    //        services.AddMvc(options => options.EnableEndpointRouting = false);
    //    }


    //    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager) 
    //    {
    //        if (env.IsDevelopment())
    //        {
    //            app.UseDeveloperExceptionPage();
    //           // SeedDatabase.Seed();
    //        }
    //        //app.UseMvcWithDefaultRoute();
    //        app.UseFileServer();
    //        app.CustomStaticFiles();
    //        //app.UseRouting();
    //        //app.UseCors();
    //        app.UseAuthentication();//Identity iþlemi içindir
    //        app.UseMvc(routes =>
    //        {
    //            routes.MapRoute(
    //                name: "adminProducts",
    //                template: "admin/products",
    //                defaults: new { controller = "Admin", action = "ProductList" }
    //            );
    //            routes.MapRoute(
    //               name: "adminProducts1",
    //               template: "admin/products/{id?}",
    //               defaults: new { controller = "Admin", action = "EditProduct" }
    //           );

    //            routes.MapRoute(
    //                name: "products",
    //                template: "products/{category?}",
    //                defaults: new { controller = "Shop", action = "List" }
    //            );

    //            routes.MapRoute(
    //              name: "card",
    //              template: "card",
    //              defaults: new { controller = "Card", action = "Index" }
    //          );
    //            routes.MapRoute(
    //             name: "checkout",
    //             template: "checkout",
    //             defaults: new { controller = "Card", action = "checkout" }
    //         );
    //            routes.MapRoute(
    //             name: "orders",
    //             template: "orders",
    //             defaults: new { controller = "Card", action = "GetOrders" }
    //         );
    //            app.UseMvc(routes =>
    //            {
    //                routes.MapRoute(
    //                    name: "default",
    //                    template: "{controller=Home}/{action=Index}/{id?}");
    //            });

    //            //app.UseEndpoints(endpoints =>
    //            //{
    //            //    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
    //            //});
    //        });

    //       // SeedIdentity.Seed(userManager, roleManager, Configuration).Wait();
    //    }
    //}
}
