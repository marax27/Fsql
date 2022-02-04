using System.Collections.Generic;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.WhenEvaluatingRelationalOperators;

public class WhenEvaluatingGreaterThan
{
    [Theory]
    [InlineData(3, 3, false)]
    [InlineData(1, 0, true)]
    [InlineData(0, 1, false)]
    [InlineData(1999, -1, true)]
    [InlineData(1e-5, 1e-3, false)]
    public void GivenNumbersOperatorReturnExpectedResult(
        double givenFirstValue, double givenOtherValue, bool expectedResult)
    {
        var givenFirst = new ConstantExpression(new NumberValueType(givenFirstValue));
        var givenOther = new ConstantExpression(new NumberValueType(givenOtherValue));

        var expression = new GreaterThanExpression(givenFirst, givenOther);

        expression.Evaluate(new StubExpressionContext(GivenContextValues))
            .Should().Be(new BooleanValueType(expectedResult));
    }

    private static readonly IReadOnlyDictionary<Identifier, BaseValueType> GivenContextValues =
        new Dictionary<Identifier, BaseValueType>
        {
            { new("name"), new StringValueType("filename.txt") },
        };
}
