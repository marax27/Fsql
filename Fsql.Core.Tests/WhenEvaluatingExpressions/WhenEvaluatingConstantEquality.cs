using System.Collections.Generic;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions;

public class WhenEvaluatingConstantEquality
{
    [Fact]
    public void GivenIdenticalNumbersReturnTrue()
    {
        var givenFirst = new NumberValueType(3.14159);
        var givenOther = new NumberValueType(3.14159);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(true));
    }

    [Fact]
    public void GivenDifferentTypesReturnFalse()
    {
        var givenFirst = new NumberValueType(3.14159);
        var givenOther = new StringValueType("3.14159");

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Fact]
    public void GivenDifferentNumbersReturnFalse()
    {
        var givenFirst = new NumberValueType(3.14159);
        var givenOther = new NumberValueType(3.14158);

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
        var givenFirst = new StringValueType(givenString);
        var givenOther = new StringValueType(givenString);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(true));
    }

    [Theory]
    [InlineData("a", "b")]
    [InlineData("a", "A")]
    [InlineData("Long string", "Long string.")]
    public void GivenDifferentStringsReturnFalse(string givenFirstString, string givenOtherString)
    {
        var givenFirst = new StringValueType(givenFirstString);
        var givenOther = new StringValueType(givenOtherString);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Fact]
    public void GivenNumberAndNullReturnFalse()
    {
        var givenFirst = new NumberValueType(-100.0);
        var givenOther = new NullValueType();

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Fact]
    public void GivenNullAndNumberReturnFalse()
    {
        var givenFirst = new NullValueType();
        var givenOther = new NumberValueType(-100.0);

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(false));
    }

    [Fact]
    public void Given2NullsReturnTrue()
    {
        var givenFirst = new NullValueType();
        var givenOther = new NullValueType();

        Act(givenFirst, givenOther)
            .Should().Be(new BooleanValueType(true));
    }

    private static BaseValueType Act(BaseValueType givenFirst, BaseValueType givenOther)
    {
        var context = new StubExpressionContext(GivenContextValues);
        var expression = new EqualsExpression(new ConstantExpression(givenFirst), new ConstantExpression(givenOther));
        return expression.Evaluate(context);
    }

    private static readonly IReadOnlyDictionary<Identifier, BaseValueType> GivenContextValues =
        new Dictionary<Identifier, BaseValueType>
        {
            { new("name"), new StringValueType("filename.txt") },
        };
}