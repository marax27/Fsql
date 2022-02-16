using System.Collections.Generic;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.RelationalOperators;

public class WhenEvaluatingAttributeConstantEquality
{
    [Theory]
    [InlineData(1234, true)]
    [InlineData(-100, false)]
    [InlineData(1235, false)]
    public void GivenIdentifierAndConstantReturnExpectedResult(double givenConstant, bool expectedResult)
    {
        var givenFirst = new IdentifierReferenceExpression(new("size"));
        var givenOther = new NumberConstant(givenConstant);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(expectedResult));
    }

    [Theory]
    [InlineData(1234, true)]
    [InlineData(-100, false)]
    [InlineData(1235, false)]
    public void GivenConstantAndIdentifierReturnExpectedResult(double givenConstant, bool expectedResult)
    {
        var givenFirst = new NumberConstant(givenConstant);
        var givenOther = new IdentifierReferenceExpression(new("size"));

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(expectedResult));
    }

    [Fact]
    public void GivenNameIdentifierAndConstantOfDifferentTypeReturnFalse()
    {
        var givenFirst = new IdentifierReferenceExpression(new("name"));
        var givenOther = new NumberConstant(1000);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Fact]
    public void GivenSizeIdentifierAndConstantOfDifferentTypeReturnFalse()
    {
        var givenFirst = new IdentifierReferenceExpression(new("size"));
        var givenOther = new StringConstant("...");

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    private static BaseValueType Act(Expression givenFirst, Expression givenOther)
    {
        var context = new StubExpressionContext(GivenContextValues);
        var expression = new EqualsExpression(givenFirst, givenOther);
        return expression.Evaluate(context);
    }

    private static readonly IReadOnlyDictionary<Identifier, BaseValueType> GivenContextValues =
        new Dictionary<Identifier, BaseValueType>
        {
            { new("name"), new StringValueType("filename.txt") },
            { new("extension"), new StringValueType(".txt") },
            { new("size"), new NumberValueType(1234) },
        };
}
