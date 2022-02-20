using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MvcClient.Consts;
using MvcClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MvcClient.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace MvcClient.Middlewares
{
    /// <summary>
    /// Her istekte Access Token'ın expire olacağı tarihi kontrol eder. Eğer Expire olduysa API'ye refresh token ile istek yollar.Kullanıcıyı MVC tarafında çıkış yaptırır ve yeni bilgiler ile tekrar giriş yaptırır. 
    /// </summary>
    public class JwtAuthentication : IMiddleware
    {
        private readonly HttpClient _apiSampleClient;
        public JwtAuthentication(IHttpClientFactory httpClientFactory)
        {
            _apiSampleClient = httpClientFactory.CreateClient(HttpClientNames.ApiSample);
        }
        public async Task InvokeAsync(HttpContext context, Microsoft.AspNetCore.Http.RequestDelegate next)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await next.Invoke(context);
            }
            else
            {
                var expireUnixDate = Convert.ToInt32(context?.User?.Claims?.First(x => x.Type == "exp")?.Value);

                var expireUtcDate = DateTimeOffset.FromUnixTimeSeconds(expireUnixDate).DateTime;

                if (expireUtcDate < DateTime.UtcNow)
                {

                    var result = await context.AuthenticateAsync();

                    object param = new
                    {
                        AccessToken = result.Properties.GetTokenValue("AccessToken"),
                        RefreshToken = result.Properties.GetTokenValue("RefreshToken"),
                    };


                    var tokenResponse = await _apiSampleClient.PostAsync<object, TokenViewModel>("https://localhost:5001/api/tokens/refresh-token", param);


                    if (tokenResponse.IsSuccedeed)
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(tokenResponse.Data.AccessToken);

                        await context.SignOutAsync("ExternalAuth");

                        await context.JwtSignInAsync("ExternalAuth", jwtToken, tokenResponse.Data.AccessToken, tokenResponse.Data.RefreshToken);
                    }
                }


                await next.Invoke(context);

            }


        }

    }
}
