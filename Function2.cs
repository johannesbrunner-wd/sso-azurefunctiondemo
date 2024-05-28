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
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, HttpResponse res)
        {
            _logger.LogInformation("C# HTTP trigger function to delete all cookies.");

            // Get all cookies from the request
            var cookies = req.Cookies;

            // Prepare a list to hold Set-Cookie headers for the response
            foreach (var cookie in cookies)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = System.DateTimeOffset.UnixEpoch,
                    Path = "/"
                };
                res.Cookies.Append(cookie.Key, "", cookieOptions);
            }

            return new OkObjectResult("All cookies deleted.");
        }
    }
}
