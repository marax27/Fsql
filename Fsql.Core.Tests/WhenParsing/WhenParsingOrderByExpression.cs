using System;
using FluentAssertions;
using Xunit;

namespace Fsql.Core.Tests.WhenParsing;

public class WhenParsingOrderByExpression : IClassFixture<ParserFixture>
{
    private readonly ParserFixture _parserFixture;

    public WhenParsingOrderByExpression(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Fact]
    public void GivenOrderBy1NamedAttributeParseSuccessfully()
    {
        Action act = () => _parserFixture.Sut.Parse("SELECT * FROM ./path ORDER BY size");

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("size")]
    [InlineData("Name")]
    [InlineData("CREATE_TIME")]
    public void GivenOrderBy1NamedAttributeReturnExpectedAttribute(string givenAttribute)
    {
        var query = _parserFixture.Sut.Parse($"SELECT * FROM ./path ORDER BY {givenAttribute}");
        var actualConditions = query.OrderByExpression.Conditions;
        actualConditions.Should().BeEquivalentTo(new[] { new OrderCondition(givenAttribute, true) });
    }

    [Theory]
    [InlineData("size")]
    [InlineData("Name")]
    [InlineData("CREATE_TIME")]
    public void GivenOrderBy1NamedAttributeWithAscendingModifierReturnExpectedAttribute(string givenAttribute)
    {
        var query = _parserFixture.Sut.Parse($"SELECT * FROM ./path ORDER BY {givenAttribute} ASC");
        var actualConditions = query.OrderByExpression.Conditions;
        actualConditions.Should().BeEquivalentTo(new[] { new OrderCondition(givenAttribute, true) });
    }

    [Theory]
    [InlineData("size")]
    [InlineData("Name")]
    [InlineData("CREATE_TIME")]
    public void GivenOrderBy1NamedAttributeWithDescendingModifierReturnExpectedAttribute(string givenAttribute)
    {
        var query = _parserFixture.Sut.Parse($"SELECT * FROM ./path ORDER BY {givenAttribute} DESC");
        var actualConditions = query.OrderByExpression.Conditions;
        actualConditions.Should().BeEquivalentTo(new[] { new OrderCondition(givenAttribute, false) });
    }
}
