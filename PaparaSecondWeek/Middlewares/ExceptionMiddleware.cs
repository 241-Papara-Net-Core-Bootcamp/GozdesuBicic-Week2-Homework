using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PaparaSecondWeek.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PaparaSecondWeek.Middlewares
{
    
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Bir sonraki pipeline'a geçmeni sağlayan metot
        public async Task Invoke(HttpContext httpContext)
        {
            // Sadece bir tane try-catch kullanarak sistem içinde handle edilmemiş bütün exceptionları handle etmiş oluyoruz.
            // Bu try-catch uygulama içindeki tüm try-catchleri yakalayacak demektir.
            try
            {
                await _next.Invoke(httpContext); //next'in Invoke metodunu çağırırsam, uygulama bu satırda normal seyrine devam edecek, yani Controller'a gidecek
            } // program içinde yakaladığı hatayı catch bölümünde yakalıyor
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }         
        }
        private static Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return httpContext.Response.WriteAsync(new ErrorResultModel()
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = ex.Message
            }.ToString());
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        // Extension'ı var, IApplicationBuilder'ı genişletmiş.
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
