using Cympatic.Stub.Connectivity.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Cympatic.Stub.Server.UnitTests.TestData
{
    public static class ResponseModelTestData
    {
        public static IEnumerable<ResponseModel> GetResponseModels()
        {
            var responseModels = new List<ResponseModel>
            {
                new ResponseModel
                {
                    Path = "segment1/segment2",
                    Query = new Dictionary<string, string>
                    {
                        { "queryparam1", "1" }
                    },
                    HttpMethods = new List<string>{ HttpMethod.Get.Method, HttpMethod.Post.Method },
                    ReturnStatusCode = HttpStatusCode.Created,
                    Result = new { CustomerId = 5, CustomerName = "Pepsi"}
                },
                new ResponseModel
                {
                    Path = "segment1/{*wildcard}/segment3",
                    Query = new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "{*wildcard}"}
                    },
                    HttpMethods = new List<string>(),
                    ReturnStatusCode = HttpStatusCode.Created,
                    Headers = new Dictionary<string, IEnumerable<string>>
                    {
                        { "Accept", new []{ "application/json" } }
                    },
                    Location = new Uri("/created/at/location/2", UriKind.Relative),
                    Result = new { CustomerId = 5, CustomerName = "Pepsi" }
                }
            };

            return responseModels;
        }
    }
}
