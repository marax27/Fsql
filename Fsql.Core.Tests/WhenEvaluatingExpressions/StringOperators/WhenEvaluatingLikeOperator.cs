using System.Collections.Generic;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.StringOperators;

public class WhenEvaluatingLikeOperator
{
    [Theory]
    [InlineData("", "", true)]
    [InlineData("sample-text", "sample-text", true)]
    [InlineData("Sample.Text.", "Sample.Text.", true)]
    [InlineData("12345678", "12345678", true)]
    [InlineData("ABC", "abc", false)]
    [InlineData("abc", "ABC", false)]
    [InlineData("123456", "12345", false)]

    [InlineData("1234", "%1234", true)]
    [InlineData("a1234", "%1234", true)]
    [InlineData("aa1234", "%1234", true)]
    [InlineData("aaa1234", "%1234", true)]
    [InlineData("Abc12341234", "%1234", true)]
    [InlineData("12341234", "%1234", true)]
    [InlineData("12345", "%1234", false)]
    [InlineData("1234aaa", "%1234", false)]
    [InlineData("12x34", "%1234", false)]
    
    [InlineData("user_salary", "user_salary", true)]
    [InlineData("user:salary", "user_salary", true)]
    [InlineData("user.salary", "user_salary", true)]
    [InlineData("user+salary", "user_salary", true)]
    [InlineData("User1Salary", "User_Salary", true)]
    [InlineData("UserSalary", "User_Salary", false)]

    [InlineData("sample0FORMAT01", "sample%FORMAT__", true)]
    [InlineData("sampleFORMAT01", "sample%FORMAT__", true)]
    [InlineData("sampleFORMATFORMAT..", "sample%FORMAT__", true)]
    [InlineData("FORMAT123sample01", "sample%FORMAT__", false)]

    [InlineData("abc123", "abc...", false)]
    [InlineData("abc...", "abc...", true)]
    [InlineData("abc123", "...123", false)]
    [InlineData("...123", "...123", true)]

    [InlineData("abc\\d+", "abc\\d+", true)]
    [InlineData("abc123456", "abc\\d+", false)]
    public void GivenSampleInputAndPatternReturnExpectedResult(string givenInput, string givenPattern, bool expectedMatch)
    {
        var expectedResult = new BooleanValueType(expectedMatch);
        var givenContext = new StubExpressionContext(new Dictionary<Identifier, BaseValueType>());
        var givenInputValue = new StringConstant(givenInput);
        var givenPatternValue = new StringConstant(givenPattern);

        var sut = new LikeOperatorExpression(givenInputValue, givenPatternValue);

        var actualResult = sut.Evaluate(givenContext);

        actualResult.Should().Be(expectedResult);
    }

    [Fact]
    public void GivenNullInputReturnNull()
    {
        var givenContext = new StubExpressionContext(new Dictionary<Identifier, BaseValueType>());
        var givenInputValue = new StubExpression(new NullValueType());
        var givenPatternValue = new StringConstant("a%");

        var sut = new LikeOperatorExpression(givenInputValue, givenPatternValue);

        var actualResult = sut.Evaluate(givenContext);

        actualResult.Should().Be(new NullValueType());
    }

    [Fact]
    public void GivenNullPatternReturnNull()
    {
        var givenContext = new StubExpressionContext(new Dictionary<Identifier, BaseValueType>());
        var givenInputValue = new StringConstant("abc");
        var givenPatternValue = new StubExpression(new NullValueType());

        var sut = new LikeOperatorExpression(givenInputValue, givenPatternValue);

        var actualResult = sut.Evaluate(givenContext);

        actualResult.Should().Be(new NullValueType());
    }
}
