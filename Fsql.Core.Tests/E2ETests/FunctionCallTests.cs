using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem.Abstractions;
using Fsql.Core.Tests.WhenEvaluating;
using Fsql.Core.Tests.WhenParsing;
using Xunit;

namespace Fsql.Core.Tests.E2ETests;

public class FunctionCallTests : IClassFixture<ParserFixture>
{
    private readonly ParserFixture _parserFixture;

    public FunctionCallTests(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Fact]
    public void WhenUsing1FunctionInWhereExpressionReturnExpectedValues()
    {
        var givenInput = "SELECT name,type FROM /home WHERE human(size) = '2.0k'";
        var evaluation = new QueryEvaluation(FileSystemAccess);

        var query = _parserFixture.Sut.Parse(givenInput);

        var actualResult = evaluation.Evaluate(query);

        actualResult.Rows.Should().HaveCount(3);
    }

    [Theory]
    [InlineData("SELECT name FROM /home WHERE lower(name) > upper(name)", 9)]
    [InlineData("SELECT name FROM /home WHERE lower(name) < upper(name)", 0)]
    public void WhenUsing2FunctionsInWhereExpressionReturnExpectedValues(string givenInput, int expectedCount)
    {
        var evaluation = new QueryEvaluation(FileSystemAccess);

        var query = _parserFixture.Sut.Parse(givenInput);

        var actualResult = evaluation.Evaluate(query);

        actualResult.Rows.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void WhenOrderingByFunctionResultReturnExpectedValues()
    {
        var givenInput = "SELECT name FROM /home ORDER BY lower(name) ASC";

        var expectedResult = new[]
        {
            new BaseValueType[]{ new StringValueType("a1.txt") },
            new BaseValueType[]{ new StringValueType("a2.jpg") },
            new BaseValueType[]{ new StringValueType("a3.mov") },
            new BaseValueType[]{ new StringValueType("AAA") },
            new BaseValueType[]{ new StringValueType("aaa1") },
            new BaseValueType[]{ new StringValueType("b1.txt") },
            new BaseValueType[]{ new StringValueType("b2.jpg") },
            new BaseValueType[]{ new StringValueType("b3.mov") },
            new BaseValueType[]{ new StringValueType("ZZZ") },
        };

        var evaluation = new QueryEvaluation(FileSystemAccess);

        var query = _parserFixture.Sut.Parse(givenInput);

        var actualResult = evaluation.Evaluate(query);

        actualResult.Rows.Should().BeEquivalentTo(expectedResult,
            o => o.ComparingRecordsByValue().WithStrictOrdering());
    }

    //[Fact]
    //public void WhenCallingFunctionInSelectExpressionReturnExpectedValues()
    //{
    //    var givenInput = "SELECT name,human(size),upper(extension) FROM /home";
    //    var query = _parserFixture.Sut.Parse(givenInput);
    //}

    //[Fact]
    //public void WhenCallingFunctionWithMixedUppercaseLowercaseCharactersReturnExpectedValues()
    //{
    //    var givenInput = "SELECT name,HUMAN(size),Upper(extension) FROM /home";
    //    var query = _parserFixture.Sut.Parse(givenInput);
    //}

    private IFileSystemAccess FileSystemAccess =>
        new FakeFileSystemAccess()
            .WithFiles("/home", 1024, "a1.txt", "a2.jpg", "a3.mov")
            .WithFiles("/home", 2048, "b1.txt", "b2.jpg", "b3.mov")
            .WithFiles("/home", 4096, "AAA", "aaa1", "ZZZ");
}
