using Cympatic.Extensions.SpecFlow.Interfaces;
using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cympatic.Extensions.SpecFlow
{
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
                var ignoredProperties = expected.GetType().GetIgnoredProperties();

                apiServiceResultValue.Value
                    .Should()
                    .BeEquivalentTo(expected, options => options
                        .IncludingAllRuntimeProperties()
                        .Excluding(model => ignoredProperties.Any(x => x == model.SelectedMemberInfo.Name))
                        .Using<string>(FluentAssertionsHelper.CompareStrings).WhenTypeIs<string>());
            }
        }
    }
}
