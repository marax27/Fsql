using FluentAssertions;
using Xunit;

namespace Fsql.Core.Tests.WhenParsing;

public class WhenParsingConvolutedQueries : IClassFixture<ParserFixture>
{
    private readonly ParserFixture _parserFixture;

    public WhenParsingConvolutedQueries(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Fact]
    public void GivenNestedParenthesesReturnExpectedQuery()
    {
        var expectedExpression = new GreaterThanExpression(
            new IdentifierReferenceExpression(new("size")),
            new NumberConstant(2000)
        );

        var givenInput = "SELECT * FROM ./path WHERE ((((size)))) > ((2000))";
        var actualResult = _parserFixture.Sut.Parse(givenInput);

        actualResult.WhereExpression.Should().Be(expectedExpression);
    }

    [Fact]
    public void GivenAndOperatorAndLikeOperatorReturnExpectedQuery()
    {
        var expectedExpression = new AndExpression(
            new EqualsExpression(
                new IdentifierReferenceExpression(new("type")),
                new StringConstant("File")
            ),
            new LikeOperatorExpression(
                new IdentifierReferenceExpression(new("name")),
                new StringConstant("%a%")
            )
        );

        var givenInput = "SELECT * FROM ./path WHERE type='File' AND name LIKE '%a%'";
        var actualResult = _parserFixture.Sut.Parse(givenInput);

        actualResult.WhereExpression.Should().Be(expectedExpression);
    }
}
