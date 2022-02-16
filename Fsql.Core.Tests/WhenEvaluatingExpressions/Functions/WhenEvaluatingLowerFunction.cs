using System;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.Functions;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.Functions;

public class WhenEvaluatingLowerFunction
{
    [Theory]
    [InlineData("Sample Text", "sample text")]
    [InlineData("ALL_UPPER", "all_upper")]
    [InlineData(":all:lower:", ":all:lower:")]
    [InlineData("  A  ", "  a  ")]
    [InlineData("", "")]
    public void GivenValidArgumentReturnExpectedValue(string given, string expected)
    {
        var expectedResult = new StringValueType(expected);
        var sut = new LowerFunction();

        var actualResult = sut.Evaluate(new[] { new StringValueType(given) });

        actualResult.Should().Be(expectedResult);
    }

    [Fact]
    public void GivenZeroArgumentsThrowExpectedException()
    {
        var sut = new LowerFunction();

        Action act = () => sut.Evaluate(Array.Empty<BaseValueType>());

        act.Should().Throw<ArgumentCountException>()
            .Which.Message.Should()
            .ContainAll("Function has received a wrong number of arguments", "Expected 1", "received 0");
    }

    [Fact]
    public void GivenNumberArgumentThrowExpectedException()
    {
        var sut = new LowerFunction();

        Action act = () => sut.Evaluate(new []{ new NumberValueType(1234) });

        act.Should().Throw<ArgumentTypeException>()
            .Which.Message.Should()
            .ContainAll("Function has received a wrong argument", "Expected <StringValueType>", "received <NumberValueType>");
    }

    [Fact]
    public void GivenNullArgumentThrowExpectedException()
    {
        var sut = new LowerFunction();

        Action act = () => sut.Evaluate(new[] { new NullValueType() });

        act.Should().Throw<ArgumentTypeException>()
            .Which.Message.Should()
            .ContainAll("Function has received a wrong argument", "Expected <StringValueType>", "received <NullValueType>");
    }
}
