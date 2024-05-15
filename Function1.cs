using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;

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
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogTrace("Start function");            
            CosmosClient client = new CosmosClient(
                accountEndpoint: Environment.GetEnvironmentVariable("CosmosDbConnectionSetting", EnvironmentVariableTarget.Process),
                new DefaultAzureCredential()
            );

            using FeedIterator<DatabaseProperties> iterator = client.GetDatabaseQueryIterator<DatabaseProperties>();

            List<(string name, string uri)> databases = new();
            while (iterator.HasMoreResults)
            {
                foreach (DatabaseProperties database in await iterator.ReadNextAsync())
                {
                    _logger.LogTrace($"[Database Found]\t{database.Id}");
                    databases.Add((database.Id, database.SelfLink));
                }
            }

            _logger.LogInformation("C# HTTP trigger function processed a request.");                        

            var myObj = new { text = "Test1" };            

            return new JsonResult(myObj);            
        }
    }
}
