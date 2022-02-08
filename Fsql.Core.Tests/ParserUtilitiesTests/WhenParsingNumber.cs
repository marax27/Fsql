using System;
using FluentAssertions;
using Fsql.Core.QueryLanguage;
using Xunit;

namespace Fsql.Core.Tests.ParserUtilitiesTests;

public class WhenParsingNumber
{
    [Theory]
    [InlineData("+3", 3.0)]
    [InlineData("-3.14", -3.14)]
    [InlineData("1024", 1024.0)]
    [InlineData("-0.015", -0.015)]
    public void GivenNoMultiplierReturnExpectedValue(string givenInput, double expectedResult)
    {
        var actualResult = ParserUtilities.ParseNumber(givenInput);
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("2k", 2048)]
    [InlineData("-2k", -2048)]
    [InlineData("2K", 2048)]
    [InlineData("-2K", -2048)]
    public void GivenKibiMultiplierReturnExpectedValue(string givenInput, double expectedResult)
    {
        var actualResult = ParserUtilities.ParseNumber(givenInput);
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("1m", 1024 * 1024)]
    [InlineData("10M", 10 * 1024 * 1024)]
    public void GivenMebiMultiplierReturnExpectedValue(string givenInput, double expectedResult)
    {
        var actualResult = ParserUtilities.ParseNumber(givenInput);
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("1g", 1024.0 * 1024.0 * 1024.0)]
    [InlineData("10G", 10 * 1024.0 * 1024.0 * 1024.0)]
    public void GivenGibiMultiplierReturnExpectedValue(string givenInput, double expectedResult)
    {
        var actualResult = ParserUtilities.ParseNumber(givenInput);
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("1t", 1024.0 * 1024.0 * 1024.0 * 1024.0)]
    [InlineData("10T", 10 * 1024.0 * 1024.0 * 1024.0 * 1024.0)]
    public void GivenTebiMultiplierReturnExpectedValue(string givenInput, double expectedResult)
    {
        var actualResult = ParserUtilities.ParseNumber(givenInput);
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("123y")]
    public void GivenInvalidMultiplierThrow(string givenInput)
    {
        Action act = () => ParserUtilities.ParseNumber(givenInput);

        act.Should().Throw<ApplicationException>();
    }
}
