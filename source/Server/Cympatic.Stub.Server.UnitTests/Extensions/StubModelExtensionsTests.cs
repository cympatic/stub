using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xunit;

namespace Cympatic.Stub.Server.UnitTests.Extensions
{
    public class StubModelExtensionsTests
    {
        [Theory]
        [MemberData(nameof(GetTestData_GetCreatedLocation))]
        public void GetCreatedLocation(string scheme, HostString host, Uri location, string expected)
        {
            var sut = new ResponseModel
            {
                Location = location
            };

            var actual = sut.GetCreatedLocation(scheme, host);

            actual.Should().Be(expected);
        }

        public static IEnumerable<object[]> GetTestData_GetCreatedLocation()
        {
            yield return new object[]
                {
                    Uri.UriSchemeHttps,
                    new HostString("localhost", 8080),
                    new Uri("segment1/segment2", UriKind.Relative),
                    "https://localhost:8080/segment1/segment2"
                };

            yield return new object[]
                {
                    Uri.UriSchemeHttps,
                    new HostString("localhost", 8080),
                    null,
                    string.Empty
                };

            yield return new object[]
                {
                    Uri.UriSchemeHttps,
                    null,
                    null,
                    string.Empty
                };

            yield return new object[]
                {
                    Uri.UriSchemeHttps,
                    new HostString("localhost"),
                    new Uri("segment1/segment2", UriKind.Relative),
                    "https://localhost/segment1/segment2"
                };

            yield return new object[]
                {
                    Uri.UriSchemeHttps,
                    null,
                    new Uri("segment1/segment2", UriKind.Relative),
                    "segment1/segment2"
                };

            yield return new object[]
                {
                    Uri.UriSchemeHttps,
                    new HostString("localhost", 8080),
                    new Uri("https://segment1/segment2", UriKind.Absolute),
                    "https://segment1/segment2"
                };
        }

        [Theory]
        [MemberData(nameof(GetTestData_IsMatching))]
        public void ResponseModel_IsMatching(IList<string> httpMethods, string httpMethod, string path, IDictionary<string, string> query, bool expected)
        {
            var sut = new ResponseModel
            {
                HttpMethods = httpMethods,
                Path = "segment1/{*wildcard}/segment3",
                Query = new Dictionary<string, string>
                {
                    { "queryparam1", "1" },
                    { "queryparam2", "{*wildcard}" }
                }
            };

            var actual = sut.IsMatching(httpMethod, path, query);

            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(GetTestData_IsMatching))]
        public void RequestModel_IsMatching(IList<string> httpMethods, string httpMethod, string path, IDictionary<string, string> query, bool expected)
        {
            var sut = new RequestModel
            {
                HttpMethod = httpMethod,
                Path = path,
                Query = query
            };

            var actual = sut.IsMatching(httpMethods, "segment1/{*wildcard}/segment3", new Dictionary<string, string>
                {
                    { "queryparam1", "1" },
                    { "queryparam2", "{*wildcard}" }
                });

            actual.Should().Be(expected);
        }

        public static IEnumerable<object[]> GetTestData_IsMatching()
        {
            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    HttpMethods.Post,
                    $"segment1/{Guid.NewGuid():N}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    true
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    HttpMethods.Get,
                    $"segment1/{Guid.NewGuid():N}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    true
                };

            yield return new object[]
                {
                    new List<string> (),
                    HttpMethods.Put,
                    $"segment1/{Guid.NewGuid():N}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    true
                };

            yield return new object[]
                {
                    null,
                    HttpMethods.Put,
                    $"segment1/{Guid.NewGuid():N}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    true
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    HttpMethods.Put,
                    $"segment1/{Guid.NewGuid():N}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    false
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    HttpMethods.Get,
                    $"segment1/{Guid.NewGuid():N}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" }
                    },
                    false
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    HttpMethods.Get,
                    $"segment1/segment2",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    false
                };
        }

        [Theory]
        [MemberData(nameof(GetTestData_AreRequestParamsEqual))]
        public void ResponseModel_AreRequestParamsEqual(IList<string> responseHttpMethods, IList<string> httpMethods, string path, IDictionary<string, string> query, bool expected)
        {
            var sut = new ResponseModel
            {
                HttpMethods = responseHttpMethods,
                Path = "segment1/{*wildcard}/segment3",
                Query = new Dictionary<string, string>
                {
                    { "queryparam1", "1" },
                    { "queryparam2", "{*wildcard}" }
                }
            };

            var actual = sut.AreRequestParamsEqual(httpMethods, path, query);

            actual.Should().Be(expected);
        }

        public static IEnumerable<object[]> GetTestData_AreRequestParamsEqual()
        {
            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    "segment1/{*wildcard}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    true
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    "segment1/{*wildcard}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" }
                    },
                    false
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    new List<string> (),
                    "segment1/{*wildcard}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    false
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    new List<string> { HttpMethods.Get },
                    "segment1/{*wildcard}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    false
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    new List<string> { HttpMethods.Put },
                    "segment1/{*wildcard}/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    false
                };

            yield return new object[]
                {
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    "segment1/segment2/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    true
                };

            yield return new object[]
                {
                    null,
                    new List<string> { HttpMethods.Get, HttpMethods.Post },
                    "segment1/segment2/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    true
                };

            yield return new object[]
                {
                    null,
                    null,
                    "segment1/segment2/segment3",
                    new Dictionary<string, string>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", "2" }
                    },
                    true
                };
        }

    }
}
