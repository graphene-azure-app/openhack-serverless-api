using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions;
using Newtonsoft.Json;
using BFYOC_api.Models;
using System.Net.Http;


namespace BFYOC_api
{
    public static class CreateRating
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "BFYOC",collectionName: "ratings",
                        ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<RatingModel> icecreamRatingOut,
            ILogger log)
        {
            string _userid = null;
            string _productid = null;

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            _userid = _userid ?? data?.userId;
            _productid = _productid ?? data?.productId;

            var result = await _httpClient.GetAsync($"https://serverlessohuser.trafficmanager.net/api/GetUser/?userid={_userid}");
            var resultContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var resultProduct = await _httpClient.GetAsync($"https://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={_productid}");
                var resultProductContent = await result.Content.ReadAsStringAsync();

                if (resultProduct.IsSuccessStatusCode)
                {
                    var newRatingGuid = Guid.NewGuid().ToString();
                    var _icecreamRating = new RatingModel
                    {
                        id = newRatingGuid,
                        userId = _userid,
                        productId = _productid,
                        locationName = data?.locationName,
                        rating = data?.rating,
                        userNotes = data?.userNotes,
                        timeStamp = DateTime.Now
                    };

                    await icecreamRatingOut.AddAsync(_icecreamRating);
                    return new OkObjectResult(_icecreamRating);
                }
            }

            return new BadRequestResult();
        }
    }
}
