using FluentAssertions;
using Fsql.Core.Evaluation;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.WhenEvaluatingRelationalOperators;

public class WhenEvaluatingValuesComparedWithNull
{
    [Theory]
    [MemberData(nameof(WhenEvaluatingValuesComparedWithNull.GetTestCases), MemberType = typeof(WhenEvaluatingValuesComparedWithNull))]
    public void TestEqualsExpression(Expression givenLeft, Expression givenRight)
    {
        var sut = new EqualsExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(new NullValueType());
    }

    [Theory]
    [MemberData(nameof(WhenEvaluatingValuesComparedWithNull.GetTestCases), MemberType = typeof(WhenEvaluatingValuesComparedWithNull))]
    public void TestNotEqualExpression(Expression givenLeft, Expression givenRight)
    {
        var sut = new NotEqualExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(new NullValueType());
    }

    [Theory]
    [MemberData(nameof(WhenEvaluatingValuesComparedWithNull.GetTestCases), MemberType = typeof(WhenEvaluatingValuesComparedWithNull))]
    public void TestLessThanExpression(Expression givenLeft, Expression givenRight)
    {
        var sut = new LessThanExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(new NullValueType());
    }

    [Theory]
    [MemberData(nameof(WhenEvaluatingValuesComparedWithNull.GetTestCases), MemberType = typeof(WhenEvaluatingValuesComparedWithNull))]
    public void TestGreaterThanExpression(Expression givenLeft, Expression givenRight)
    {
        var sut = new GreaterThanExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(new NullValueType());
    }

    [Theory]
    [MemberData(nameof(WhenEvaluatingValuesComparedWithNull.GetTestCases), MemberType = typeof(WhenEvaluatingValuesComparedWithNull))]
    public void TestLessThanOrEqualExpression(Expression givenLeft, Expression givenRight)
    {
        var sut = new LessThanOrEqualExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(new NullValueType());
    }

    [Theory]
    [MemberData(nameof(WhenEvaluatingValuesComparedWithNull.GetTestCases), MemberType = typeof(WhenEvaluatingValuesComparedWithNull))]
    public void TestGreaterThanOrEqualExpression(Expression givenLeft, Expression givenRight)
    {
        var sut = new GreaterThanOrEqualExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(new NullValueType());
    }

    private static IEnumerable<object[]> GetTestCases()
    {
        return DataTypeComparisonDatasets.ValuesAgainstNull
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right)})
        .Concat(
            DataTypeComparisonDatasets.ValuesAgainstNull
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left) })
        );
    }

}
