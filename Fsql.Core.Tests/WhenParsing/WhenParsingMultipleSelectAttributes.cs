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
    public void GivenWildcardAttributeThenParseSuccessfully()
    {
        var result = _parserFixture.Sut.Parse("SELECT * FROM ./path");
        result.SelectedAttributes.Should().BeEquivalentTo(new[] { "*" });
    }

}