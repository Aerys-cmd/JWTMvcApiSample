using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcClient.Consts;
using MvcClient.Extensions;
using MvcClient.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient
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
            services.AddControllersWithViews();
            services.AddHttpClient(HttpClientNames.ApiSample, opt =>
            {
                opt.BaseAddress = new Uri("https://localhost:5001");
                // DefaultRequestHeaders �zerinden https://localhost:5001 sunucuna her istekde g�nderilecek olan de�erler
                opt.DefaultRequestHeaders.Add("User-Agent", "MVC App");
                opt.DefaultRequestHeaders.Add("client_id", "client-mvc");
                opt.DefaultRequestHeaders.Add("client_secret", "x-client");

                // her istekde header eklenmesi gereken de�erler.

            });


            services.AddAuthentication("ExternalAuth").AddCookie("ExternalAuth", opt =>
            {

                opt.Cookie.Name = "ExternalCookie";
                opt.Cookie.HttpOnly = false;
                opt.LoginPath = "/Account/Login";
                opt.LogoutPath = "/Account/Logout";
                //opt.ExpireTimeSpan = TimeSpan.FromSeconds(1900);
                //opt.SlidingExpiration = true;
                opt.AccessDeniedPath = "/Account/AccessDenied";


            });
            services.AddTransient<JwtAuthentication>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseJwtAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
