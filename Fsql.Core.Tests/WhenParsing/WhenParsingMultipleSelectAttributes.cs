using FluentAssertions;
using Xunit;

namespace Fsql.Core.Tests.WhenParsing;

public class WhenParsingMultipleSelectAttributes : IClassFixture<ParserFixture>
{
    private readonly ParserFixture _parserFixture;

    public WhenParsingMultipleSelectAttributes(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Fact]
    public void GivenWildcardAttributeThenReturnExpectedAttributes()
    {
        var result = _parserFixture.Sut.Parse("SELECT * FROM ./path");
        result.SelectedAttributes.Should().BeEquivalentTo(new IdentifierReferenceExpression[] { new(new("*")) });
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("__999")]
    [InlineData("named_attribute")]
    [InlineData("CapitalCase01234")]
    public void Given1NamedAttributeThenReturnExpectedAttributes(string givenAttributeName)
    {
        var result = _parserFixture.Sut.Parse($"SELECT {givenAttributeName} FROM ./path");
        result.SelectedAttributes.Should().BeEquivalentTo(new IdentifierReferenceExpression[] { new(new(givenAttributeName)) });
    }

    [Fact]
    public void Given2NamedAttributesThenReturnExpectedAttributes()
    {
        var result = _parserFixture.Sut.Parse("SELECT alpha, bravo FROM path");
        result.SelectedAttributes.Should().BeEquivalentTo(new IdentifierReferenceExpression[] { new(new("alpha")), new(new("bravo")) },
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenMixedWildcardAndNamedAttributesThenReturnExpectedAttributes()
    {
        var result = _parserFixture.Sut.Parse("SELECT *, middle, * FROM path");
        result.SelectedAttributes.Should().BeEquivalentTo(new IdentifierReferenceExpression[] { new(new("*")), new(new("middle")), new(new("*")) },
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenWildcardTwiceThenReturnExpectedAttributes()
    {
        var result = _parserFixture.Sut.Parse("SELECT *,* FROM path");
        result.SelectedAttributes.Should().BeEquivalentTo(new IdentifierReferenceExpression[] { new(new("*")), new(new("*")) },
            o => o.WithStrictOrdering());
    }
}