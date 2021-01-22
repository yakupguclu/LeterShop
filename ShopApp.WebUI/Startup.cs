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

        //Configuration daki inject i�lemi appsettings.json k�las�ndaki bilgilere eri�mek i�in yaz�l�r
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Identity s�n�flar�m�z buradaki  IdentityConnection metodu ile database ekleniyor.
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
                options.Password.RequireLowercase = true;//mutlaka k���k harf girilmeli;
                options.Password.RequireUppercase = true;//mutlaka b�y�k harf girilmeli;
                options.Password.RequiredLength = 6; //mutlaka 6 karakter olmal�
                options.Password.RequireNonAlphanumeric = false; //alfa numerik girmek zorunda olmaz

                options.Lockout.MaxFailedAccessAttempts = 5; // en fazla be� kere yanl�� �ifre deneyebilir.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // 5 dakika sonra tekrar �ifre deneyebilir

                // options.User.AllowedUserNameCharacters = ""; // kullan�c� ad� i�erisinde al�nmas� istenmeyen karkterler  �rnek "�,�,�,�,�".
                options.User.RequireUniqueEmail = true; // ayn� mail ile bir daha  hesap olu�turmaz

                options.SignIn.RequireConfirmedEmail = true; //mail ile do�rulama ger�ekle�tirir.
                options.SignIn.RequireConfirmedPhoneNumber = false;//telefon do�rulamas� kapal�
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login"; //login sayfas�n�n yolu
                options.LogoutPath = "/account/logout";//��k� sayfas�
                options.AccessDeniedPath = "/account/accesdenied";//yetkisi olmayan bir yere girdi�inde y�nlendirilecek sayfa.
                options.SlidingExpiration = true; //kullan�c�n�n cookie s�resi belli s�rede biter ve giri� ister
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //Cookie nin saklanma s�resi 60dk. Not: Varsay�lan� 20dk 
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true, //scripler okunamaz g�venlik a��s�ndan �nemli
                    Name = "ShopApp.Securty.Cookie", //Cookie nin �zel ismi
                    SameSite = SameSiteMode.Strict //Ba�ka bir kullan�c� bizim cookie yi al�p server a g�nderemez. Csrf 
                };
            });

            //hem businesteki hem de dataacces d�n al�nan hata unutma
            //Unable to resolve service for type 'ShopApp.Business.Abstract.IReviewService' while attempting to activate 'ShopApp.WebUI.Controllers.CardController'.tarz�nda  hata verir

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
            app.UseAuthentication();//Identity i�lemi i�indir
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

    //    //Configuration daki inject i�lemi appsettings.json k�las�ndaki bilgilere eri�mek i�in yaz�l�r
    //   public IConfiguration   Configuration { get; set; }
    //    public Startup(IConfiguration configuration)
    //    {
    //        Configuration = configuration;
    //    }

    //    // This method gets called by the runtime. Use this method to add services to the container.
    //    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    //    public void ConfigureServices(IServiceCollection services)  
    //    {
    //        //Identity s�n�flar�m�z buradaki  IdentityConnection metodu ile database ekleniyor.
    //        services.AddDbContext<ApplicationIdentityDbContext>(options =>
    //        options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));
    //        services.AddIdentity<ApplicationUser, IdentityRole>()
    //            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    //            .AddDefaultTokenProviders();
    //        services.Configure<IdentityOptions>(options =>
    //        {
    //            //password
    //            options.Password.RequireDigit = true;//mutlaka numerik karakter girilmeli
    //            options.Password.RequireLowercase = true;//mutlaka k���k harf girilmeli;
    //            options.Password.RequireUppercase = true;//mutlaka b�y�k harf girilmeli;
    //            options.Password.RequiredLength = 6; //mutlaka 6 karakter olmal�
    //            options.Password.RequireNonAlphanumeric = false; //alfa numerik girmek zorunda olmaz

    //            options.Lockout.MaxFailedAccessAttempts = 5; // en fazla be� kere yanl�� �ifre deneyebilir.
    //            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // 5 dakika sonra tekrar �ifre deneyebilir

    //            // options.User.AllowedUserNameCharacters = ""; // kullan�c� ad� i�erisinde al�nmas� istenmeyen karkterler  �rnek "�,�,�,�,�".
    //            options.User.RequireUniqueEmail = true; // ayn� mail ile bir daha  hesap olu�turmaz

    //            options.SignIn.RequireConfirmedEmail = true; //mail ile do�rulama ger�ekle�tirir.
    //            options.SignIn.RequireConfirmedPhoneNumber = false;//telefon do�rulamas� kapal�
    //        });

    //        services.ConfigureApplicationCookie(options =>
    //        {
    //            options.LoginPath = "/account/login"; //login sayfas�n�n yolu
    //            options.LogoutPath = "/account/logout";//��k� sayfas�
    //            options.AccessDeniedPath = "/account/accesdenied";//yetkisi olmayan bir yere girdi�inde y�nlendirilecek sayfa.
    //            options.SlidingExpiration = true; //kullan�c�n�n cookie s�resi belli s�rede biter ve giri� ister
    //            options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //Cookie nin saklanma s�resi 60dk. Not: Varsay�lan� 20dk 
    //            options.Cookie = new CookieBuilder
    //            {
    //                HttpOnly = true, //scripler okunamaz g�venlik a��s�ndan �nemli
    //                Name = "ShopApp.Securty.Cookie", //Cookie nin �zel ismi
    //                SameSite=SameSiteMode.Strict //Ba�ka bir kullan�c� bizim cookie yi al�p server a g�nderemez. Csrf 
    //            };
    //        });

    //        //Not Burda hem businesteki hem de dataacces teki s�n�flar� tan�mlamal�yoz yoksa  Unable to resolve service for type 'ShopApp.Business.Abstract.IReviewService' while attempting to activate 'ShopApp.WebUI.Controllers.CardController'.tarz�nda  hata verir

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
    //        app.UseAuthentication();//Identity i�lemi i�indir
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
