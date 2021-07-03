using Cympatic.Stub.Abstractions.Models;
using Cympatic.Stub.Server.Containers;
using Cympatic.Stub.Server.UnitTests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace Cympatic.Stub.Server.UnitTests.Containers
{
    public class ResponseModelContainerTests
    {
        private readonly ResponseModelContainer _sut;
        private readonly string _identifierValue;

        public ResponseModelContainerTests()
        {
            _sut = new ResponseModelContainer(Mock.Of<ILogger<ResponseModelContainer>>());
            _identifierValue = Guid.NewGuid().ToString("N");
        }

        [Fact]
        public void AddRequest()
        {
            var expected = ResponseModelTestData.GetResponseModels();

            _sut.AddOrUpdate(_identifierValue, expected);

            var actual = _sut.Get(_identifierValue);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Get_WithItems()
        {
            var expected = PrepareContainer(_identifierValue);

            var actual = _sut.Get(_identifierValue);

            actual.Should().HaveCount(expected.Count());
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Get_WithNoItems()
        {
            var actual = _sut.Get(_identifierValue);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public void Remove()
        {
            PrepareContainer(_identifierValue);

            _sut.Remove(_identifierValue);
            var actual = _sut.Get(_identifierValue);

            actual.Should().HaveCount(0);
        }

        [Theory]
        [MemberData(nameof(GetTestDataFind))]
        public void Find(string httpMethod, string path, Dictionary<string, StringValues> query, ResponseModel expectedModel)
        {
            PrepareContainer(_identifierValue);

            var actual = _sut.FindResult(_identifierValue, httpMethod, path, new QueryCollection(query));

            actual.Should().BeEquivalentTo(expectedModel);
        }

        public static IEnumerable<object[]> GetTestDataFind()
        {
            var models = ResponseModelTestData.GetResponseModels().ToArray();

            yield return new object[]
                {
                    HttpMethod.Post.Method,
                    "segment1/segment2",
                    new Dictionary<string, StringValues>
                    {
                        { "queryparam1", "1" }
                    },
                    models[0]
                };

            yield return new object[]
                {
                    HttpMethod.Post.Method,
                    "segment1/segment2",
                    new Dictionary<string, StringValues>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", Guid.NewGuid().ToString("N") }
                    },
                    default
                };

            yield return new object[]
                {
                    HttpMethod.Post.Method,
                    "segment1/segment2",
                    new Dictionary<string, StringValues>
                    {
                        { "queryparam1", "2" }
                    },
                    default
                };

            yield return new object[]
                {
                    HttpMethod.Put.Method,
                    "segment1/segment2",
                    new Dictionary<string, StringValues>
                    {
                        { "queryparam1", "1" }
                    },
                    default
                };

            yield return new object[]
                {
                    HttpMethod.Get.Method,
                    $"segment1/{Guid.NewGuid().ToString("N")}/segment3",
                    new Dictionary<string, StringValues>
                    {
                        { "queryparam1", "1" },
                        { "queryparam2", Guid.NewGuid().ToString("N") }
                    },
                    models[1]
                };
        }

        private IEnumerable<ResponseModel> PrepareContainer(string identifierValue)
        {
            var models = ResponseModelTestData.GetResponseModels();
            _sut.AddOrUpdate(identifierValue, models);

            return models;
        }
    }
}
