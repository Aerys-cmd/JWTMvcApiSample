using ApiSample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ApiSample
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
            services.AddHttpContextAccessor();

            services.AddTransient<ITokenService, JwtTokenService>();
            // buras� �ok �nemli sak�n singleton yapmayal�m.
            services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
           

            //services.AddAuthentication("adminScheme").AddJwtBearer()

            services.AddControllers();
            // OPEN API
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiSample", Version = "v1" });
            });


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;// token sessionda tutumam�z� sa�lar
                //opt.Audience = Configuration["JWT:audience"];

                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true, // yanl�� audince g�nderirse token kabul etme
                    ValidateIssuer = true, // access tokendan yanl�� issuer gelirse validate etme
                    ValidateIssuerSigningKey = true, // �ok �nemli signkey validate etmemiz laz�m
                    ValidateLifetime = true, // token ya�am s�resini kontrol et
                    ValidIssuer = Configuration["JWT:issuer"], // valid issuer de�eri
                    ValidAudience = Configuration["JWT:audience"], // valid audience de�eri
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:signingKey"])),

                };
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiSample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication(); // uygulama kimlik do�rulama uygulas�n
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
