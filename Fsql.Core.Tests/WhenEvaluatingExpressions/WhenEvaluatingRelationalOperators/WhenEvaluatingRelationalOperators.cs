using FluentAssertions;
using Fsql.Core.Evaluation;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.WhenEvaluatingRelationalOperators;

public class WhenEvaluatingRelationalOperators
{
    [Theory]
    [MemberData(nameof(GetEqualsTestCases), MemberType = typeof(WhenEvaluatingRelationalOperators))]
    public void TestEqualsExpression(Expression givenLeft, Expression givenRight, bool areEqual)
    {
        var expectedResult = new BooleanValueType(areEqual);
        var sut = new EqualsExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(GetNotEqualTestCases), MemberType = typeof(WhenEvaluatingRelationalOperators))]
    public void TestNotEqualExpression(Expression givenLeft, Expression givenRight, bool areNotEqual)
    {
        var expectedResult = new BooleanValueType(areNotEqual);
        var sut = new NotEqualExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(GetLessThanTestCases), MemberType = typeof(WhenEvaluatingRelationalOperators))]
    public void TestLessThanExpression(Expression givenLeft, Expression givenRight, bool leftSmaller)
    {
        var expectedResult = new BooleanValueType(leftSmaller);
        var sut = new LessThanExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(GetGreaterThanTestCases), MemberType = typeof(WhenEvaluatingRelationalOperators))]
    public void TestGreaterThanExpression(Expression givenLeft, Expression givenRight, bool leftGreater)
    {
        var expectedResult = new BooleanValueType(leftGreater);
        var sut = new GreaterThanExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(GetLessThanOrEqualTestCases), MemberType = typeof(WhenEvaluatingRelationalOperators))]
    public void TestLessThanOrEqualExpression(Expression givenLeft, Expression givenRight, bool leftLessOrEqual)
    {
        var expectedResult = new BooleanValueType(leftLessOrEqual);
        var sut = new LessThanOrEqualExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(GetGreaterThanOrEqualTestCases), MemberType = typeof(WhenEvaluatingRelationalOperators))]
    public void TestGreaterThanOrEqualExpression(Expression givenLeft, Expression givenRight, bool leftGreaterOrEqual)
    {
        var expectedResult = new BooleanValueType(leftGreaterOrEqual);
        var sut = new GreaterThanOrEqualExpression(givenLeft, givenRight);
        var actualResult = sut.Evaluate(new StubExpressionContext(new Dictionary<Identifier, BaseValueType>()));
        actualResult.Should().Be(expectedResult);
    }

    private static IEnumerable<object[]> GetEqualsTestCases()
    {
        return 
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), true })
        .Concat(
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), true })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), false })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), false })
        );
    }

    private static IEnumerable<object[]> GetNotEqualTestCases()
    {
        return
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), false })
        .Concat(
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), false })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), true })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), true })
        );
    }

    private static IEnumerable<object[]> GetLessThanTestCases()
    {
        return
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), false })
        .Concat(
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), false })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), true })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), false })
        );
    }

    private static IEnumerable<object[]> GetGreaterThanTestCases()
    {
        return
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), false })
        .Concat(
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), false })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), false })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), true })
        );
    }

    private static IEnumerable<object[]> GetLessThanOrEqualTestCases()
    {
        return
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), true })
        .Concat(
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), true })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), true })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), false })
        );
    }

    private static IEnumerable<object[]> GetGreaterThanOrEqualTestCases()
    {
        return
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), true })
        .Concat(
            DataTypeComparisonDatasets.EqualValuesOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), true })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Left), new StubExpression(testCase.Right), false })
        )
        .Concat(
            DataTypeComparisonDatasets.FirstValueSmallerOfTheSameType
            .Select(testCase => new object[] { new StubExpression(testCase.Right), new StubExpression(testCase.Left), true })
        );
    }
}
