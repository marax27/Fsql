using FluentAssertions;
using Xunit;

namespace Fsql.Core.Tests.WhenParsing;

public class WhenParsingWhereExpression : IClassFixture<ParserFixture>
{
    private readonly ParserFixture _parserFixture;

    public WhenParsingWhereExpression(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Fact]
    public void GivenSampleWhereExpressionParseSuccessfully()
    {
        var query = _parserFixture.Sut.Parse("Select * FROM ./path WHERE name = 'a.txt'");
        query.Should().NotBeNull();
    }

    [Fact]
    public void GivenSampleWhereExpressionContainExpectedExpression()
    {
        var query = _parserFixture.Sut.Parse("Select * FROM ./path WHERE name = 'a.txt'");
        var whereExpression = query.WhereExpression;

        whereExpression.Should().Be(new EqualsExpression(
            new IdentifierReferenceExpression(new("name")),
            new StringConstant("a.txt")
        ));
    }
}
