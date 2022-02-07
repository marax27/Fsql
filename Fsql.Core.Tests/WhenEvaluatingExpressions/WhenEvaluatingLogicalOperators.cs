using System.Collections.Generic;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions;

public class WhenEvaluatingLogicalOperators
{
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void Given2BooleanValuesWhenEvaluatingAndExpressionReturnExpectedResult(bool givenLeft, bool givenRight, bool expectedResult)
    {
        var left = new BooleanValueType(givenLeft);
        var right = new BooleanValueType(givenRight);
        var sut = new AndExpression(new StubExpression(left), new StubExpression(right));

        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));

        actualResult.Should().Be(new BooleanValueType(expectedResult));
    }

    [Fact]
    public void GivenTrueAndNullWhenEvaluatingAndExpressionReturnNull()
    {
        var left = new BooleanValueType(true);
        var right = new NullValueType();
        var sut = new AndExpression(new StubExpression(left), new StubExpression(right));

        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));

        actualResult.Should().Be(new NullValueType());
    }

    [Fact]
    public void GivenFalseAndNullWhenEvaluatingAndExpressionReturnFalse()
    {
        var left = new BooleanValueType(false);
        var right = new NullValueType();
        var sut = new AndExpression(new StubExpression(left), new StubExpression(right));

        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));

        actualResult.Should().Be(new BooleanValueType(false));
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    public void TestOrOperator(bool givenLeft, bool givenRight, bool expectedResult)
    {
        var left = new BooleanValueType(givenLeft);
        var right = new BooleanValueType(givenRight);
        var sut = new OrExpression(new StubExpression(left), new StubExpression(right));

        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));

        actualResult.Should().Be(new BooleanValueType(expectedResult));
    }

    [Fact]
    public void GivenTrueAndNullArgumentWhenEvaluatingOrExpressionReturnTrue()
    {
        var left = new BooleanValueType(true);
        var right = new NullValueType();
        var sut = new OrExpression(new StubExpression(left), new StubExpression(right));

        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));

        actualResult.Should().Be(new BooleanValueType(true));
    }

    [Fact]
    public void GivenFalseAndNullArgumentWhenEvaluatingOrExpressionReturnNull()
    {
        var left = new BooleanValueType(false);
        var right = new NullValueType();
        var sut = new OrExpression(new StubExpression(left), new StubExpression(right));

        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));

        actualResult.Should().Be(new NullValueType());
    }
}
