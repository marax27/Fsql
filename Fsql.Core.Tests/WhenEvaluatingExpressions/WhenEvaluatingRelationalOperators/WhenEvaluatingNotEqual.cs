using System.Collections.Generic;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.WhenEvaluatingRelationalOperators;

public class WhenEvaluatingNotEqual
{
    [Theory]
    [InlineData(3, 3, false)]
    [InlineData(0, 0, false)]
    [InlineData(-1, +1, true)]
    [InlineData(123.0, 123.01, true)]
    public void GivenNumbersAndNotEqualOperatorReturnExpectedResult(
        double givenFirstValue, double givenOtherValue, bool expectedResult)
    {
        var givenFirst = new NumberConstant(givenFirstValue);
        var givenOther = new NumberConstant(givenOtherValue);

        var expression = new NotEqualExpression(givenFirst, givenOther);

        expression.Evaluate(new StubExpressionContext(GivenContextValues))
            .Should().Be(new BooleanValueType(expectedResult));
    }

    [Theory]
    [InlineData("", "", false)]
    [InlineData("example.", "example.", false)]
    [InlineData("Sample_Text", "sample_text", true)]
    [InlineData("", "---", true)]
    [InlineData("---", "", true)]
    public void GivenStringsAndNotEqualOperatorReturnExpectedResult(
        string givenFirstValue, string givenOtherValue, bool expectedResult)
    {
        var givenFirst = new StringConstant(givenFirstValue);
        var givenOther = new StringConstant(givenOtherValue);

        var expression = new NotEqualExpression(givenFirst, givenOther);

        expression.Evaluate(new StubExpressionContext(GivenContextValues))
            .Should().Be(new BooleanValueType(expectedResult));
    }

    private static readonly IReadOnlyDictionary<Identifier, BaseValueType> GivenContextValues =
        new Dictionary<Identifier, BaseValueType>
        {
            { new("name"), new StringValueType("filename.txt") },
        };
}
