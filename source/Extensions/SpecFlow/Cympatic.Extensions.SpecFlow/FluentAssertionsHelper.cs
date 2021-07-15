using FluentAssertions.Equivalency;
using FluentAssertions.Execution;

namespace Cympatic.Extensions.SpecFlow
{
    public static class FluentAssertionsHelper
    {
        public static void CompareStrings(IAssertionContext<string> ctx)
        {
            var equal = (ctx.Subject ?? string.Empty).Equals(ctx.Expectation ?? string.Empty);

            Execute.Assertion
                .BecauseOf(ctx.Because, ctx.BecauseArgs)
                .ForCondition(equal)
                .FailWith("Expected {context:string} to be {0}{reason}, but found {1}", ctx.Subject, ctx.Expectation);
        }
    }
}
