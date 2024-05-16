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
        //https://learn.microsoft.com/en-us/azure/cosmos-db/managed-identity-based-authentication
        //https://learn.microsoft.com/de-de/azure/cosmos-db/mongodb/how-to-setup-rbac
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

            // Get a reference to the database and container
            Database database1 = client.GetDatabase("my-database");
            Container container = database1.GetContainer("my-container");

            // Define the query
            var queryDefinition = new QueryDefinition("SELECT * FROM c");

            // Execute the query
            using (var iterator2 = container.GetItemQueryIterator<CosmosDocument>(queryDefinition))
            {
                while (iterator2.HasMoreResults)
                {
                    var response = await iterator2.ReadNextAsync();
                    foreach (var item in response)
                    {
                        Console.WriteLine($"Item ID: {item.Id}, Name: {item.Msg}");
                    }
                }
            }            

            _logger.LogInformation("C# HTTP trigger function processed a request.");                        

            var myObj = new { text = "Test1" };            

            return new JsonResult(myObj);            
        }
    }
}
