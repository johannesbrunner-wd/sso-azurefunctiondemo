using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;

namespace FunctionApp1
{   
    public class HttpExample1
    {
        private readonly ILogger<HttpExample> _logger;

        public HttpExample1(ILogger<HttpExample> logger)
        {
            _logger = logger;
        }
        //https://learn.microsoft.com/en-us/azure/cosmos-db/managed-identity-based-authentication
        //https://learn.microsoft.com/de-de/azure/cosmos-db/mongodb/how-to-setup-rbac
        [Function("HttpExample1")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function to delete all cookies.");
            
            // A little non logical way to actually get the HttpResponse (from the HttpRequest and its HttpContext)
            req.HttpContext.Response.Cookies.Delete("AppServiceAuthSession", new CookieOptions
            {
                Domain = "gray-river-0d65c6203.5.azurestaticapps.net",
                Path = "/",
                Expires = DateTime.UnixEpoch,
                HttpOnly = true, 
                Secure = true,
                SameSite = SameSiteMode.None               
            });
            req.HttpContext.Response.Cookies.Delete("AppServiceAuthSession1", new CookieOptions
            {
                Domain = "",
                Path = "/",
                Expires = DateTime.UnixEpoch,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            req.HttpContext.Response.Cookies.Delete("StaticWebAppsAuthCookie", new CookieOptions
            {
                Domain = "",
                Path = "/",
                Expires = DateTime.UnixEpoch,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            req.HttpContext.Response.Cookies.Append("JbrCookie", DateTime.UtcNow.ToShortTimeString(), new CookieOptions
            {                
                Path = "/",
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return new OkObjectResult("Cookie set");
        }
    }
}
