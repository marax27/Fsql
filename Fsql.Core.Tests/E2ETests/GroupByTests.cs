using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem.Abstractions;
using Fsql.Core.Tests.WhenEvaluating;
using Fsql.Core.Tests.WhenParsing;
using Xunit;

namespace Fsql.Core.Tests.E2ETests;

public class GroupByTests : IClassFixture<ParserFixture>
{
    private readonly ParserFixture _parserFixture;

    public GroupByTests(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Theory]
    [InlineData("extension", 4)]
    [InlineData("size", 3)]
    [InlineData("name", 9)]
    public void WhenGroupingReturnExpectedRowCount(string givenAttribute, int expectedCount)
    {
        var givenInput = $"SELECT {givenAttribute}, COUNT(name) FROM /home GROUP BY {givenAttribute}";

        var actualResult = Act(givenInput);

        actualResult.Rows.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void WhenGroupingBySizeReturnExpectedResult()
    {
        var expectedResult = new []
        {
            new BaseValueType[] { new NumberValueType(3), new NumberValueType(4096) },
            new BaseValueType[] { new NumberValueType(3), new NumberValueType(2048) },
            new BaseValueType[] { new NumberValueType(3), new NumberValueType(1024) },
        };
        var givenInput = "SELECT count(name), size FROM /home GROUP BY size ORDER BY size DESC";
        
        var actualResult = Act(givenInput);

        actualResult.Rows.Should()
            .BeEquivalentTo(expectedResult, o => o.WithStrictOrdering().ComparingRecordsByValue());
    }

    [Fact]
    public void WhenTryingToAccessUnaggregatedKeyThrowExpectedException()
    {
        var givenInput = "SELECT count(name), extension FROM /home GROUP BY size";

        var act = () => Act(givenInput);

        act.Should().Throw<AggregateAttributeException>();
    }

    [Fact]
    public void WhenGroupingByFunctionResultReturnExpectedResult()
    {
        var givenInput = "SELECT count(name), UPPER(extension) FROM /home GROUP BY upper(extension)";

        var actualResult = Act(givenInput);

        actualResult.Rows.Should().HaveCount(4);
    }

    [Fact]
    public void WhenGroupingByFunctionResultAndSelectingAttributeThrowExpectedException()
    {
        var givenInput = "SELECT count(name), extension FROM /home GROUP BY upper(extension)";

        var act = () => Act(givenInput);

        act.Should().Throw<AggregateAttributeException>();
    }

    private QueryEvaluationResult Act(string givenInput)
    {
        var evaluation = new QueryEvaluation(FileSystemAccess);
        var query = _parserFixture.Sut.Parse(givenInput);
        return evaluation.Evaluate(query);
    }

    private IFileSystemAccess FileSystemAccess =>
        new FakeFileSystemAccess()
            .WithFiles("/home", 1024, "a1.txt", "a2.jpg", "a3.mov")
            .WithFiles("/home", 2048, "b1.txt", "b2.jpg", "b3.mov")
            .WithFiles("/home", 4096, "AAA", "aaa1", "ZZZ");
}
