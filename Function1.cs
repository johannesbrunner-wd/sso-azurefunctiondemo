using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FunctionApp1
{
    public class CosmosDocument
    {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class HttpExample
    {
        private readonly ILogger<HttpExample> _logger;

        public HttpExample(ILogger<HttpExample> logger)
        {
            _logger = logger;
        }

        [Function("HttpExample")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
            [CosmosDBInput(databaseName: "my-database", containerName: "my-container", Connection  = "CosmosDbConnectionSetting", SqlQuery = "SELECT * from c")] IEnumerable<CosmosDocument> items)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");                        

            var myObj = new { text = items.First().Msg };            

            return new JsonResult(myObj);            
        }
    }
}
