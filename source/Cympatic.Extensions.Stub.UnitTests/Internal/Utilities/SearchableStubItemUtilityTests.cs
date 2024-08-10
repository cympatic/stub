using Cympatic.Extensions.Stub.Internal.Utilities;
using FluentAssertions;
using System.Text.Json;
using Xunit.Abstractions;

namespace Cympatic.Extensions.Stub.UnitTests.Internal.Utilities;

public class SearchableStubItemUtilityTests
{
    public class IsMatchingTestCase : IXunitSerializable
    {
        public string Description { get; set; } = string.Empty;

        public IList<string> HttpMethods { get; set; } = [];
        
        public string HttpMethod { get; set; } = string.Empty;
        
        public bool Result { get; set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Description = info.GetValue<string>(nameof(Description));
            HttpMethods = info.GetValue<string[]>(nameof(HttpMethods));
            HttpMethod = info.GetValue<string>(nameof(HttpMethod));
            Result = info.GetValue<bool>(nameof(Result));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Description), Description, typeof(string));
            info.AddValue(nameof(HttpMethods), HttpMethods, typeof(string[]));
            info.AddValue(nameof(HttpMethod), HttpMethod, typeof(string));
            info.AddValue(nameof(Result), Result, typeof(bool));
        }

        public override string ToString() => Description;
    }

    public static TheoryData<IsMatchingTestCase> IsMatchingTestData()
    {
        string[] httpMethods = [Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")];

        return new TheoryData<IsMatchingTestCase>
        {
            // Both null
            { new() { Description = "Both null", HttpMethods = default!, HttpMethod = default!, Result = true } },

            // List is null and httpMethod has a value
            { new() { Description = "List is null and httpMethod has a value", HttpMethods = default!, HttpMethod = Guid.NewGuid().ToString("N"), Result = true } },

            // List is not null and httpMethod has a value not matching any
            { new() { Description = "List is not null and httpMethod has a value not matching any", HttpMethods = httpMethods, HttpMethod = Guid.NewGuid().ToString("N"), Result = false } },

            // List is not null and httpMethod is null
            { new() { Description = "List is not null and httpMethod is null", HttpMethods = httpMethods, HttpMethod = default!, Result = false } },

            // List is not null and httpMethod is empty
            { new() { Description = "List is not null and httpMethod is empty", HttpMethods = httpMethods, HttpMethod = string.Empty, Result = false } },

            // List is not null and httpMethod has a value matching 
            { new() { Description = "List is not null and httpMethod has a value matching", HttpMethods = httpMethods, HttpMethod = httpMethods[Random.Shared.Next(httpMethods.Length)], Result = true } },

            // List is not null and httpMethod has a value matching in uppercase
            { new() { Description = "List is not null and httpMethod has a value matching in uppercase", HttpMethods = httpMethods, HttpMethod = httpMethods[Random.Shared.Next(httpMethods.Length)].ToUpper(), Result = true } }
        };
    }

    [Theory]
    [MemberData(nameof(IsMatchingTestData))]
    public void IsMatching(IsMatchingTestCase testCase)
    {
        // Arrange & Act
        var actual = SearchableStubItemUtility.IsMatching(testCase.HttpMethods, testCase.HttpMethod, string.Empty, string.Empty, new Dictionary<string, string>(), new Dictionary<string, string>());

        // Assert
        actual.Should().Be(testCase.Result);
    }

    public class ComparePathTestCase : IXunitSerializable
    {
        public string Description { get; set; } = string.Empty;

        public string Current { get; set; } = string.Empty;

        public string Other { get; set; } = string.Empty;

        public bool Result { get; set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Description = info.GetValue<string>(nameof(Description));
            Current = info.GetValue<string>(nameof(Current));
            Other = info.GetValue<string>(nameof(Other));
            Result = info.GetValue<bool>(nameof(Result));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Description), Description, typeof(string));
            info.AddValue(nameof(Current), Current, typeof(string));
            info.AddValue(nameof(Other), Other, typeof(string));
            info.AddValue(nameof(Result), Result, typeof(bool));
        }

        public override string ToString() => Description;
    }

    public static TheoryData<ComparePathTestCase> ComparePathTestData()
    {
        string[] keys = [Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")];

        return new TheoryData<ComparePathTestCase>
        {
            // Both null
            { new() { Description = "Both null", Current = default!, Other = default!, Result = true } },

            // One null, one empty
            { new() { Description = "One null, one empty", Current = default!, Other = string.Empty, Result = true } },

            // both empty
            { new() { Description = "both empty", Current = string.Empty, Other = string.Empty, Result = true } },

            // Unequal count of items without wildcards
            {
                new()
                {
                    Description = "Unequal count of segments without wildcards",
                    Current = $@"{keys[0]}/{keys[1]}/{keys[2]}",
                    Other = $@"{keys[0]}/{keys[1]}",
                    Result = false
                }
            },

            // Both equal segments
            {
                new()
                {
                    Description = "Both equal segments",
                    Current = $@"{keys[0]}/{keys[1]}/{keys[2]}",
                    Other = $@"{keys[0]}/{keys[1]}/{keys[2]}",
                    Result = true
                }
            },

            // Both equal segments, one in uppercase
            {
                new()
                {
                    Description = "Both equal segments, one in uppercase",
                    Current = $@"{keys[0]}/{keys[1]}/{keys[2]}",
                    Other = $@"{keys[0].ToUpper()}/{keys[1].ToUpper()}/{keys[2].ToUpper()}",
                    Result = true
                }
            },

            // Not all segments equal without values with wildcards
            {
                new()
                {
                    Description = "Not all segments equal without wildcards",
                    Current = $@"{Guid.NewGuid():N}/{Guid.NewGuid():N}/{Guid.NewGuid():N}",
                    Other = $@"{Guid.NewGuid():N}/{Guid.NewGuid():N}/{Guid.NewGuid():N}",
                    Result = false
                }
            },

            // Not all segments equal with values with wildcards
            {
                new()
                {
                    Description = "Not all segments equal with values with wildcards",
                    Current = $@"{keys[0]}/{{*}}/{keys[2]}",
                    Other = $@"{keys[0]}/{Guid.NewGuid():N}/{keys[2]}",
                    Result = true
                }
            }
        };
    }

    [Theory]
    [MemberData(nameof(ComparePathTestData))]
    public void ComparePath(ComparePathTestCase testCase)
    {
        // Arrange & Act
        var actual = SearchableStubItemUtility.ComparePath(testCase.Current, testCase.Other);

        // Assert
        actual.Should().Be(testCase.Result);
    }

    public class CompareQueryTestCase : IXunitSerializable
    {
        public string Description { get; set; } = string.Empty;

        public Dictionary<string, string> Current { get; set; } = [];

        public Dictionary<string, string> Other { get; set; } = [];
        
        public bool Result { get; set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Description = info.GetValue<string>(nameof(Description));
            Current = JsonSerializer.Deserialize<Dictionary<string, string>>(info.GetValue<string>(nameof(Current)))!;
            Other = JsonSerializer.Deserialize<Dictionary<string, string>>(info.GetValue<string>(nameof(Other)))!;
            Result = info.GetValue<bool>(nameof(Result));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Description), Description, typeof(string));
            info.AddValue(nameof(Current), JsonSerializer.Serialize(Current), typeof(string));
            info.AddValue(nameof(Other), JsonSerializer.Serialize(Other), typeof(string));
            info.AddValue(nameof(Result), Result, typeof(bool));
        }

        public override string ToString() => Description;
    }

    public static TheoryData<CompareQueryTestCase> CompareQueryTestData()
    {
        string[] keys = [Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")];
        string[] values = [Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")];

        return new TheoryData<CompareQueryTestCase>
        { 
            // Both null
            {new() { Description = "Both null", Current = default!, Other = default!, Result = true } },

            // One null, one empty
            {new() { Description =  "One null, one empty", Current = default!, Other = [], Result = true } },

            // both empty
            {new() { Description =  "both empty", Current = [], Other = [], Result = true } },

            // Unequal count of items without wildcards
            {
                new()
                {
                    Description = "Unequal count of items without wildcards",
                    Current = new Dictionary<string, string>
                    {
                        { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
                        { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
                        { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") }
                    },
                    Other = new Dictionary<string, string>
                    {
                        { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
                        { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") }
                    },
                    Result = false
                }
            },

            // Both equal items
            {
                new()
                {
                    Description = "Both equal items",
                    Current = new Dictionary<string, string>
                    {
                        { keys[0], values[0] },
                        { keys[1], values[1] },
                        { keys[2], values[2] }
                    },
                    Other = new Dictionary<string, string>
                    {
                        { keys[0], values[0] },
                        { keys[1], values[1] },
                        { keys[2], values[2] }
                    },
                    Result = true
                }
            },

            // Both equal items, one in uppercase
            {
                new()
                {
                    Description = "Both equal items, one in uppercase",
                    Current = new Dictionary<string, string>
                    {
                        { keys[0], values[0] },
                        { keys[1], values[1] },
                        { keys[2], values[2] }
                    },
                    Other = new Dictionary<string, string>
                    {
                        { keys[0], values[0].ToUpper() },
                        { keys[1], values[1].ToUpper() },
                        { keys[2], values[2].ToUpper() }
                    },
                    Result = true
                }
            },

            // Both equal keys without values with wildcards
            {
                new()
                {
                    Description = "Both equal keys without values with wildcards",
                    Current = new Dictionary<string, string>
                    {
                        { keys[0], Guid.NewGuid().ToString("N") },
                        { keys[1], Guid.NewGuid().ToString("N") },
                        { keys[2], Guid.NewGuid().ToString("N") }
                    },
                    Other = new Dictionary<string, string>
                    {
                        { keys[0], Guid.NewGuid().ToString("N") },
                        { keys[1], Guid.NewGuid().ToString("N") },
                        { keys[2], Guid.NewGuid().ToString("N") }
                    },
                    Result = false
                }
            },

            // Not all keys equal without values with wildcards
            {
                new()
                {
                    Description = "Not all keys equal without values with wildcards",
                    Current = new Dictionary<string, string>
                    {
                        { keys[0], Guid.NewGuid().ToString("N") },
                        { keys[1], Guid.NewGuid().ToString("N") },
                        { keys[2], Guid.NewGuid().ToString("N") }
                    },
                    Other = new Dictionary<string, string>
                    {
                        { keys[0], Guid.NewGuid().ToString("N") },
                        { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
                        { keys[2], Guid.NewGuid().ToString("N") }
                    },
                    Result = false
                }
            },

            // Both equal keys with values with wildcards
            {
                new()
                {
                    Description = "Both equal keys with values with wildcards",
                    Current = new Dictionary<string, string>
                    {
                        { keys[0], "{*}" },
                        { keys[1], "{*}" },
                        { keys[2], "{*}" }
                    },
                    Other = new Dictionary<string, string>
                    {
                        { keys[0], Guid.NewGuid().ToString("N") },
                        { keys[1], Guid.NewGuid().ToString("N") },
                        { keys[2], Guid.NewGuid().ToString("N") }
                    },
                    Result = true
                }
            },

            // Both equal keys (one in uppercase) with values with wildcards
            {
                new()
                {
                    Description = "Both equal keys (one in uppercase) with values with wildcards",
                    Current = new Dictionary<string, string>
                    {
                        { keys[0], "{*}" },
                        { keys[1], "{*}" },
                        { keys[2], "{*}" }
                    },
                    Other = new Dictionary<string, string>
                    {
                        { keys[0].ToUpper(), Guid.NewGuid().ToString("N") },
                        { keys[1].ToUpper(), Guid.NewGuid().ToString("N") },
                        { keys[2].ToUpper(), Guid.NewGuid().ToString("N") }
                    },
                    Result = true
                }
            }
        };
    }

    [Theory]
    [MemberData(nameof(CompareQueryTestData))]
    public void CompareQuery(CompareQueryTestCase testCase)
    {
        // Arrange & Act
        var actual = SearchableStubItemUtility.CompareQuery(testCase.Current, testCase.Other);

        // Assert
        actual.Should().Be(testCase.Result);
    }
}
