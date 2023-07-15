using Cympatic.Extensions.Http.Interfaces;
using FluentAssertions;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cympatic.Extensions.Stub.SpecFlow;

public static class ApiServiceResultExtensions
{
    public static void ValidateResult(this IApiServiceResult result, [NotNull] object expected)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (result is IApiServiceResultValue apiServiceResultValue)
        {
            var expectedType = GetExpectedType(apiServiceResultValue.Value);

            var ignoredProperties = expectedType?.GetIgnoredProperties() ?? Array.Empty<string>();

            apiServiceResultValue.Value
                .Should()
                .BeEquivalentTo(expected, options => options
                    .IncludingAllRuntimeProperties()
                    .Excluding(model => ignoredProperties.Any(x => x == model.Name))
                    .Using<string>(FluentAssertionsHelper.CompareStrings).WhenTypeIs<string>());
        }
    }

    private static Type GetExpectedType(object expected)
    {
        var type = expected.GetType();
        if (!type.IsGenericType)
        {
            return type;
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return type.GenericTypeArguments.FirstOrDefault();
        }

        return null;
    }
}
