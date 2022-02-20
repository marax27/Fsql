using System;
using FluentAssertions;
using Fsql.Core.QueryLanguage;
using Xunit;

namespace Fsql.Core.Tests.WhenParsing;

[Collection("Parser test collection")]
public class WhenParsingRecursiveKeyword
{
    private readonly ParserFixture _parserFixture;

    public WhenParsingRecursiveKeyword(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Theory]
    [InlineData("Select * From c:/documents")]
    [InlineData("Select * From ./recursive")]
    [InlineData("Select * From NotRecursive")]
    public void GivenNoRecursiveKeywordModelIsNotRecursive(string givenQueryCode)
    {
        var query = _parserFixture.Sut.Parse(givenQueryCode);
        
        query.FromExpression.Recursive.Should().BeFalse();
    }

    [Theory]
    [InlineData("Select * From c:/documents Recursive")]
    [InlineData("Select * From ./recursive recursive")]
    [InlineData("Select * From . RECURSIVE")]
    public void GivenRecursiveKeywordModelIsRecursive(string givenQueryCode)
    {
        var query = _parserFixture.Sut.Parse(givenQueryCode);

        query.FromExpression.Recursive.Should().BeTrue();
    }

    [Theory]
    [InlineData("Select * From Recursive")]
    [InlineData("Select * From ../ Where size > 5000 Recursive")]
    public void GivenRecursiveKeywordInWrongPlace(string givenQueryCode)
    {
        Action act = () => _parserFixture.Sut.Parse(givenQueryCode);

        act.Should().Throw<ParserException>();
    }
}
