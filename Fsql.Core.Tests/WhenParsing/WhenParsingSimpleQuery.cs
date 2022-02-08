using System;
using FluentAssertions;
using Fsql.Core.QueryLanguage;
using Xunit;

namespace Fsql.Core.Tests.WhenParsing;

public class WhenParsingSimpleQuery : IClassFixture<ParserFixture>
{
    private readonly ParserFixture _parserFixture;

    public WhenParsingSimpleQuery(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Fact]
    public void GivenSimpleQueryThenParseSuccessfully()
    {
        var result = _parserFixture.Sut.Parse($"SELECT * FROM 'sample-path'");
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("sample_path")]
    [InlineData("/very/deep/path")]
    [InlineData("c:/x1.y2-01.PATH")]
    [InlineData("./path")]
    public void GivenSimplePathThenReturnExpectedPath(string givenPath)
    {
        var result = _parserFixture.Sut.Parse($"SELECT * FROM {givenPath}");
        result.FromExpression.Path.Should().Be(givenPath);
    }

    [Theory]
    [InlineData("'../single-quoted-path'", "../single-quoted-path")]
    [InlineData("\"2x.QuotedPath/\"", "2x.QuotedPath/")]
    public void GivenQuotedPathReturnExpectedPath(string givenPath, string expectedPath)
    {
        var result = _parserFixture.Sut.Parse($"SELECT * FROM {givenPath}");
        result.FromExpression.Path.Should().Be(expectedPath);
    }

    [Fact]
    public void GivenWildcardInPlaceOfFromPathThrowExpectedException()
    {
        Action act = () => _parserFixture.Sut.Parse("SELECT * FROM *");
        act.Should().Throw<ParserException>()
            .Which.Message.Should().Contain("Invalid token Wildcard");
    }
}