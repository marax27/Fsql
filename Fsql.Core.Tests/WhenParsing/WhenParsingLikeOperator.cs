using FluentAssertions;
using Xunit;

namespace Fsql.Core.Tests.WhenParsing;

[Collection("Parser test collection")]
public class WhenParsingLikeOperator
{
    private readonly ParserFixture _parserFixture;

    public WhenParsingLikeOperator(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Theory]
    [InlineData("Select * From /home/Documents Where (name LIKE 'abc')")]
    [InlineData("Select * From /home/Documents Where (name Like 'abc')")]
    [InlineData("Select * From /home/Documents Where (name like 'abc')")]
    public void GivenLikeOperatorReturnExpectedWhereExpression(string givenInput)
    {
        var expectedResult = new LikeOperatorExpression(
            new IdentifierReferenceExpression(new("name")),
            new StringConstant("abc")
        );

        var actualResult = _parserFixture.Sut.Parse(givenInput);

        actualResult.WhereExpression.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("Select * From /home/Documents Where (name NOT LIKE 'abc')")]
    [InlineData("Select * From /home/Documents Where (name Not Like 'abc')")]
    [InlineData("Select * From /home/Documents Where (name not like 'abc')")]
    public void GivenNotLikeOperatorReturnExpectedWhereExpression(string givenInput)
    {
        var expectedResult = new NotLikeOperatorExpression(
            new IdentifierReferenceExpression(new("name")),
            new StringConstant("abc")
        );

        var actualResult = _parserFixture.Sut.Parse(givenInput);

        actualResult.WhereExpression.Should().Be(expectedResult);
    }
}
