using Cympatic.Extensions.Stub.Internal.Utilities;
using FluentAssertions;

namespace Cympatic.Extensions.Stub.UnitTests.Internal.Utilities;

public class SearchableStubbedHttpItemUtilityTests
{
    [Theory]
    [MemberData(nameof(IsMatchingTestData))]
    public void IsMatching(string _, IList<string> httpMethods, string httpMethod, bool result)
    {
        // Arrange & Act
        var actual = SearchableStubbedHttpItemUtility.IsMatching(httpMethods!, httpMethod!, string.Empty, string.Empty, new Dictionary<string, string>(), new Dictionary<string, string>());

        // Assert
        actual.Should().Be(result);
    }

    public static TheoryData<string, IList<string>, string, bool> IsMatchingTestData()
    {
        string[] httpMethods = [Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")];
        var random = new Random();

        return new TheoryData<string, IList<string>, string, bool>
        {
            // Both null
            { "Both null", default!, default!, true },

            // List is null and httpMethod has a value
            { "List is null and httpMethod has a value", default!, Guid.NewGuid().ToString("N"), true },

            // List is not null and httpMethod has a value not matching any
            { "List is not null and httpMethod has a value not matching any", httpMethods, Guid.NewGuid().ToString("N"), false },

            // List is not null and httpMethod is null
            { "List is not null and httpMethod is null", httpMethods, default!, false },

            // List is not null and httpMethod is empty
            { "List is not null and httpMethod is empty", httpMethods, string.Empty, false },

            // List is not null and httpMethod has a value matching 
            { "List is not null and httpMethod has a value matching", httpMethods, httpMethods[random.Next(httpMethods.Length)], true },

            // List is not null and httpMethod has a value matching in uppercase
            { "List is not null and httpMethod has a value matching in uppercase", httpMethods, httpMethods[random.Next(httpMethods.Length)].ToUpper(), true }
        };
    }

    [Theory]
    [MemberData(nameof(ComparePathTestData))]
    public void ComparePath(string _, string current, string other, bool result)
    {
        // Arrange & Act
        var actual = SearchableStubbedHttpItemUtility.ComparePath(current!, other!);

        // Assert
        actual.Should().Be(result);
    }

    public static TheoryData<string, string, string, bool> ComparePathTestData()
    {
        string[] keys = [Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")];

        return new TheoryData<string, string, string, bool>
        {
            // Both null
            { "Both null", default!, default!, true },

            // One null, one empty
            { "One null, one empty", default!, string.Empty, true },

            // both empty
            { "both empty", string.Empty, string.Empty, true },

            // Unequal count of items without wildcards
            {
                "Unequal count of segments without wildcards",
                $@"{keys[0]}/{keys[1]}/{keys[2]}",
                $@"{keys[0]}/{keys[1]}",
                false
            },

            // Both equal segments
            {
                "Both equal segments",
                $@"{keys[0]}/{keys[1]}/{keys[2]}",
                $@"{keys[0]}/{keys[1]}/{keys[2]}",
                true
            },

            // Both equal segments, one in uppercase
            {
                "Both equal segments, one in uppercase",
                $@"{keys[0]}/{keys[1]}/{keys[2]}",
                $@"{keys[0].ToUpper()}/{keys[1].ToUpper()}/{keys[2].ToUpper()}",
                true
            },

            // Not all segments equal without values with wildcards
            {
                "Not all segments equal without wildcards",
                $@"{Guid.NewGuid():N}/{Guid.NewGuid():N}/{Guid.NewGuid():N}",
                $@"{Guid.NewGuid():N}/{Guid.NewGuid():N}/{Guid.NewGuid():N}",
                false
            },

            // Not all segments equal with values with wildcards
            {
                "Not all segments equal with values with wildcards",
                $@"{keys[0]}/{{*}}/{keys[2]}",
                $@"{keys[0]}/{Guid.NewGuid():N}/{keys[2]}",
                true
            },
        };
    }


    [Theory]
    [MemberData(nameof(CompareQueryTestData))]
    public void CompareQuery(string _, Dictionary<string, string> current, Dictionary<string, string> other, bool result)
    {
        // Arrange & Act
        var actual = SearchableStubbedHttpItemUtility.CompareQuery(current!, other!);

        // Assert
        actual.Should().Be(result);
    }

    public static TheoryData<string, Dictionary<string, string>, Dictionary<string, string>, bool> CompareQueryTestData()
    {
        string[] keys = [Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")];
        string[] values = [Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")];

        return new TheoryData<string, Dictionary<string, string>, Dictionary<string, string>, bool>
        { 
            // Both null
            {"Both null", default!, default!, true },

            // One null, one empty
            { "One null, one empty", default!, [], true },

            // both empty
            { "both empty", [], [], true },

            // Unequal count of items without wildcards
            {
                "Unequal count of items without wildcards",
                new Dictionary<string, string>
                {
                    { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
                    { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
                    { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") }
                },
                new Dictionary<string, string>
                {
                    { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
                    { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") }
                }, false
            },

            // Both equal items
            {
                "Both equal items",
                new Dictionary<string, string>
                {
                    { keys[0], values[0] },
                    { keys[1], values[1] },
                    { keys[2], values[2] }
                },
                new Dictionary<string, string>
                {
                    { keys[0], values[0] },
                    { keys[1], values[1] },
                    { keys[2], values[2] }
                }, true
            },

            // Both equal items, one in uppercase
            {
                "Both equal items, one in uppercase",
                new Dictionary<string, string>
                {
                    { keys[0], values[0] },
                    { keys[1], values[1] },
                    { keys[2], values[2] }
                },
                new Dictionary<string, string>
                {
                    { keys[0], values[0].ToUpper() },
                    { keys[1], values[1].ToUpper() },
                    { keys[2], values[2].ToUpper() }
                }, true
            },

            // Both equal keys without values with wildcards
            {
                "Both equal keys without values with wildcards",
                new Dictionary<string, string>
                {
                    { keys[0], Guid.NewGuid().ToString("N") },
                    { keys[1], Guid.NewGuid().ToString("N") },
                    { keys[2], Guid.NewGuid().ToString("N") }
                },
                new Dictionary<string, string>
                {
                    { keys[0], Guid.NewGuid().ToString("N") },
                    { keys[1], Guid.NewGuid().ToString("N") },
                    { keys[2], Guid.NewGuid().ToString("N") }
                }, false
            },

            // Not all keys equal without values with wildcards
            {
                "Not all keys equal without values with wildcards",
                new Dictionary<string, string>
                {
                    { keys[0], Guid.NewGuid().ToString("N") },
                    { keys[1], Guid.NewGuid().ToString("N") },
                    { keys[2], Guid.NewGuid().ToString("N") }
                },
                new Dictionary<string, string>
                {
                    { keys[0], Guid.NewGuid().ToString("N") },
                    { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
                    { keys[2], Guid.NewGuid().ToString("N") }
                }, false
            },

            // Both equal keys with values with wildcards
            {
                "Both equal keys with values with wildcards",
                new Dictionary<string, string>
                {
                    { keys[0], "{*}" },
                    { keys[1], "{*}" },
                    { keys[2], "{*}" }
                },
                new Dictionary<string, string>
                {
                    { keys[0], Guid.NewGuid().ToString("N") },
                    { keys[1], Guid.NewGuid().ToString("N") },
                    { keys[2], Guid.NewGuid().ToString("N") }
                }, true
            },

            // Both equal keys (one in uppercase) with values with wildcards
            {
                "Both equal keys (one in uppercase) with values with wildcards",
                new Dictionary<string, string>
                {
                    { keys[0], "{*}" },
                    { keys[1], "{*}" },
                    { keys[2], "{*}" }
                },
                new Dictionary<string, string>
                {
                    { keys[0].ToUpper(), Guid.NewGuid().ToString("N") },
                    { keys[1].ToUpper(), Guid.NewGuid().ToString("N") },
                    { keys[2].ToUpper(), Guid.NewGuid().ToString("N") }
                }, true
            }
        };
    }
}
