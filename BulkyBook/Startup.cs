using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using BulkyBook.Utility;
using Stripe;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using BulkyBook.DataAccess.Initializer;

namespace BulkyBook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //Add role
            services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            //SendGrid
            services.AddSingleton<IEmailSender, EmailSender>();
            //temp data alerts
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            //SendGrid
            services.Configure<EmailOptions>(Configuration);
            //Stripe payment (set the StripeSettings.cs first)
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            //BrainTree payment - not my part
            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            //SMS function
            services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));
            //BrainTree payment- not my part
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
            //in order to use unit of work, so all controller can use it
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //first time to run the web, initialize the admin account
            services.AddScoped<IDbInitializer, DbInitializer>();
            //Install Razor RunTime Compilation
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddRazorPages();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "771655520254614";
                options.AppSecret = "73c79c339d03171676b20f69c7141893";
            });

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "839613119911-l4hfb9hn52845569pu19oj9vrtg374fv.apps.googleusercontent.com";
                options.ClientSecret = "AWnYZxWDlqRmAVBw6p6BfaG-";
            });
            //session - before add this, we created a SessionExtension class
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //Stripe SecretKey is from StripeSettings.cs from appsettings.json
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
            //add session
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            //default Admin account
            dbInitializer.Initialize();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
