using System;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.Functions;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.Functions;

public class WhenEvaluatingHumanFunction
{
    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    [InlineData(128)]
    [InlineData(999)]
    [InlineData(1023)]
    public void GivenNumberBelow1KReturnExpectedResult(int givenNumber)
    {
        var sut = new HumanFunction();
        var actualResult = sut.Evaluate(new[] { new NumberValueType(givenNumber) });
        actualResult.Should().Be(new StringValueType($"{givenNumber}"));
    }

    [Theory]
    [InlineData(118932482, "113M")]
    [InlineData(1024, "1.0k")]
    [InlineData(1536, "1.5k")]
    [InlineData(2040, "2.0k")]
    [InlineData(1024 * 10, "10k")]
    [InlineData(1024 * 999, "999k")]
    [InlineData(1024 * 1000, "1000k")]
    [InlineData(1024 * 1023, "1023k")]
    [InlineData(1024 * 1024, "1.0M")]
    [InlineData(1024 * 1024 * 5, "5.0M")]
    [InlineData(1024 * 1024 * 1023, "1023M")]
    [InlineData(1024.0 * 1024.0 * 1024.0, "1.0G")]
    [InlineData(1024.0 * 1024.0 * 1024.0 * 1024.0, "1.0T")]
    [InlineData(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, "inf")]
    public void GivenNumberAbove1KReturnExpectedResult(double givenNumber, string expectedResult)
    {
        var sut = new HumanFunction();
        var actualResult = sut.Evaluate(new[] { new NumberValueType(givenNumber) });
        actualResult.Should().Be(new StringValueType(expectedResult));
    }

    [Fact]
    public void GivenZeroArgumentsThrowExpectedException()
    {
        var sut = new HumanFunction();

        Action act = () => sut.Evaluate(Array.Empty<BaseValueType>());

        act.Should().Throw<ArgumentCountException>()
            .Which.Message.Should()
            .ContainAll("Function has received a wrong number of arguments", "Expected 1", "received 0");
    }

    [Fact]
    public void Given2ArgumentsThrowExpectedException()
    {
        var givenArguments = new[]
        {
            new NumberValueType(123), new NumberValueType(345)
        };
        var sut = new HumanFunction();

        Action act = () => sut.Evaluate(givenArguments);

        act.Should().Throw<ArgumentCountException>()
            .Which.Message.Should()
            .ContainAll("Function has received a wrong number of arguments", "Expected 1", "received 2");
    }

    [Fact]
    public void GivenStringArgumentThrowExpectedException()
    {
        var givenArguments = new[] { new StringValueType("123") };
        var sut = new HumanFunction();

        Action act = () => sut.Evaluate(givenArguments);

        act.Should().Throw<ArgumentTypeException>()
            .Which.Message.Should()
            .ContainAll("Function has received a wrong argument", "Expected <NumberValueType>", "received <StringValueType>");
    }

    [Fact]
    public void GivenDateTimeArgumentThrowExpectedException()
    {
        var givenArguments = new[] { new DateTimeValueType(DateTime.MinValue) };
        var sut = new HumanFunction();

        Action act = () => sut.Evaluate(givenArguments);

        act.Should().Throw<ArgumentTypeException>()
            .Which.Message.Should()
            .ContainAll("Function has received a wrong argument", "Expected <NumberValueType>", "received <DateTimeValueType>");
    }

    [Fact]
    public void GivenNullArgumentThrowExpectedException()
    {
        var givenArguments = new[] { new NullValueType() };
        var sut = new HumanFunction();

        Action act = () => sut.Evaluate(givenArguments);

        act.Should().Throw<ArgumentTypeException>()
            .Which.Message.Should()
            .ContainAll("Function has received a wrong argument", "Expected <NumberValueType>", "received <NullValueType>");
    }
}
