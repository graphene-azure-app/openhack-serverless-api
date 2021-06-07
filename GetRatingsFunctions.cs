using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace BFYOC_api
{
    public static class GetRatingsFunctions
    {
        [FunctionName(nameof(GetRatings))]
        public static IActionResult GetRatings(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "BFYOC", collectionName: "ratings", ConnectionStringSetting = "CosmosDBConnection")] IEnumerable<JObject> allRatings)
        {
            string userId = null;

            if (req.GetQueryParameterDictionary()?.TryGetValue(@"userId", out userId) == true
                && !string.IsNullOrWhiteSpace(userId))
            {
                var userRatings = allRatings.Where(r => r.Value<string>(@"userId") == userId);

                return !userRatings.Any() ? new NotFoundObjectResult($@"No ratings found for user '{userId}'") : (IActionResult)new OkObjectResult(userRatings);

            }
            else
            {
                return new BadRequestObjectResult(@"userId is required as a query parameter");
            }
        }
    }
}

