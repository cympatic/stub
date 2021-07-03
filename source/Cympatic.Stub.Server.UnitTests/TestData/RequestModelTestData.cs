using Cympatic.Stub.Abstractions.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace Cympatic.Stub.Server.UnitTests.TestData
{
    public static class RequestModelTestData
    {
        public static RequestModel GetRequestModel()
        {
            return new RequestModel
            {
                Path = "segment1/wildcard/segment3",
                Query = new Dictionary<string, string>
                {
                    { "queryparam1", "2" }
                },
                HttpMethod = HttpMethod.Get.Method,
                Body = JsonSerializer.Serialize(new { CustomerId = 5, CustomerName = "Pepsi" }),
                ResponseFound = true
            };
        }

        public static IEnumerable<RequestModel> GetRequestModels()
        {
            var requestModels = new List<RequestModel>
            {
                new RequestModel
                {
                    Path = "segment1/wildcard/segment3",
                    Query = new Dictionary<string, string>
                    {
                        { "queryparam1", "1" }
                    },
                    HttpMethod = HttpMethod.Get.Method,
                    Body = JsonSerializer.Serialize(new { CustomerId = 5, CustomerName = "Pepsi"}),
                    ResponseFound = true
                },
                new RequestModel
                {
                    Path = "segment1/wildcard/segment3",
                    Query = new Dictionary<string, string>
                    {
                        { "queryparam1", "1" }
                    },
                    HttpMethod = HttpMethod.Post.Method,
                    Headers = new Dictionary<string, IEnumerable<string>>
                    {
                        { "Accept", new []{ "application/json" } }
                    },
                    Body = JsonSerializer.Serialize(new { CustomerId = 5, CustomerName = "Pepsi"}),
                    ResponseFound = true
                }
            };

            return requestModels;
        }
    }
}
