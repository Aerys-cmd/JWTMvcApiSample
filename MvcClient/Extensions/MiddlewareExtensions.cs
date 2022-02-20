using Microsoft.AspNetCore.Builder;
using MvcClient.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Extensions
{
    public static class MiddlewareExtensions
    {
      
            public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<JwtAuthentication>();
            }
        
    }
}
