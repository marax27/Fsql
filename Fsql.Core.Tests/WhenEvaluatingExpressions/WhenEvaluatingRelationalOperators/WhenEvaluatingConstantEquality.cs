using System.Collections.Generic;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.WhenEvaluatingRelationalOperators;

public class WhenEvaluatingConstantEquality
{
    [Fact]
    public void GivenIdenticalNumbersReturnTrue()
    {
        var givenFirst = new NumberConstant(3.14159);
        var givenOther = new NumberConstant(3.14159);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(true));
    }

    [Fact]
    public void GivenDifferentTypesReturnFalse()
    {
        var givenFirst = new NumberConstant(3.14159);
        var givenOther = new StringConstant("3.14159");

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Theory]
    [InlineData(3.14159, 3.14158)]
    public void GivenDifferentNumbersReturnFalse(double givenFirstValue, double givenOtherValue)
    {
        var givenFirst = new NumberConstant(givenFirstValue);
        var givenOther = new NumberConstant(givenOtherValue);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Theory]
    [InlineData("first")]
    [InlineData("Sample long string.")]
    [InlineData("/complex string WITH **different** ch4r4ct3rs./")]
    [InlineData("")]
    [InlineData("    ")]
    public void GivenIdenticalStringsReturnTrue(string givenString)
    {
        var givenFirst = new StringConstant(givenString);
        var givenOther = new StringConstant(givenString);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(true));
    }

    [Theory]
    [InlineData("a", "b")]
    [InlineData("a", "A")]
    [InlineData("Long string", "Long string.")]
    public void GivenDifferentStringsReturnFalse(string givenFirstString, string givenOtherString)
    {
        var givenFirst = new StringConstant(givenFirstString);
        var givenOther = new StringConstant(givenOtherString);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Fact]
    public void GivenNumberAndNullReturnFalse()
    {
        var givenFirst = new NumberConstant(-100.0);
        var givenOther = new NullConstant();

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Fact]
    public void GivenNullAndNumberReturnFalse()
    {
        var givenFirst = new NullConstant();
        var givenOther = new NumberConstant(-100.0);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Fact]
    public void Given2NullsReturnTrue()
    {
        var givenFirst = new NullConstant();
        var givenOther = new NullConstant();

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(true));
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
        };
}
