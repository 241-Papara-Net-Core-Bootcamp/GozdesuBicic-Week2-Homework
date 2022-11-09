using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PaparaSecondWeek.Middlewares;
using PaparaSecondWeek.Models;
using PaparaSecondWeek.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PaparaSecondWeek
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaparaSecondWeek", Version = "v1" });
            });

            //Ge�ici. Servis her istenildi�inde yeni instance al�n�r.
            services.AddTransient<IOwnerServices, OwnerServices>();
            //Kapsaml�. Yeni bir istek geldi�inde obje olu�turulur.
            //services.AddScoped<IOwnerServices, OwnerServices>();
            // Tek. �lk istek geldi�inde bir tane instance sonra gelen t�m istekler ayn� instance �zerinden
            //devam ettirilir.
            //services.AddSingleton<IOwnerServices, OwnerServices>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /* Middleware: Request - Response - Pipeline
         * Biz Client olarak istekte bulununca Controller'a d��er. Middleware araya giriyor.
         * Request'e herhangi bir loglama i�lemi yapabilir miyim diye bak�yor. �rn: Cacheleme, loglama, yetkilendirme kontrol� ve bunun sonunda kullan�c�ya bir �ey d�nebilir miyim diye bak�yor. Orta Katmanda denir.Middleware'ler Configure alt�nda tutulur.
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaparaSecondWeek v1"));
            }
           
            // Middlewares, Use ile ba�larlar. Request'i handle etmemizi yani manip�le etmemizi sa�l�yor.
            app.UseHttpsRedirection();

            app.UseMiddleware<ExceptionMiddleware>(); // ExceptionMiddleWare ekledim.

            app.UseRouting(); // Route i�lemini yapan Middleware.
            app.UseHeaderCheckMiddleware();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           

            //app.UseMiddleware<HeaderCheckMiddleware>(); //2. farkl� register etme y�ntemi
        }
    }
}
