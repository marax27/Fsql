using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.Tests.WhenEvaluating;
using Fsql.Core.Tests.WhenParsing;
using Xunit;

namespace Fsql.Core.Tests.E2ETests;

[Collection("Parser test collection")]
public class SimpleQueryWithOrderingTests
{
    private readonly ParserFixture _parserFixture;

    public SimpleQueryWithOrderingTests(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Fact]
    public void Test1()
    {
        var givenInput = "SELECT name, extension, size FROM /home ORDER BY size ASC";

        var fsAccess = new FakeFileSystemAccess()
            .WithFiles("/home", 178, "file1.txt")
            .WithFiles("/home", 91000, "file2.jpg")
            .WithDirectories("/home", 0, "subDirectory");

        var evaluation = new QueryEvaluation(fsAccess);

        var expectedResult = new[]
        {
            new BaseValueType[]
            {
                new StringValueType("subDirectory"),
                new NullValueType(),
                new NumberValueType(0)
            },
            new BaseValueType[]
            {
                new StringValueType("file1.txt"),
                new StringValueType(".txt"),
                new NumberValueType(178)
            },
            new BaseValueType[]
            {
                new StringValueType("file2.jpg"),
                new StringValueType(".jpg"),
                new NumberValueType(91000)
            }
        };

        var query = _parserFixture.Sut.Parse(givenInput);

        var actualResult = evaluation.Evaluate(query).Rows.ToList();

        actualResult.Should().BeEquivalentTo(expectedResult,
            o => o.ComparingRecordsByValue().WithStrictOrdering());
    }

    [Fact]
    public void Test2()
    {
        var givenInput = "SELECT name, extension, size FROM /home RECURSIVE ORDER BY name";

        var fsAccess = new FakeFileSystemAccess()
            .WithFiles("/home", 178, "file1.txt")
            .WithFiles("/home", 91000, "file2.jpg")
            .WithDirectories("/home", 300, "subDirectory")
            .WithFiles("/home/subDirectory", 150, "a", "b");

        var evaluation = new QueryEvaluation(fsAccess);

        var expectedResult = new[]
        {
            new BaseValueType[]
            {
                new StringValueType("a"),
                new StringValueType(""),
                new NumberValueType(150)
            },
            new BaseValueType[]
            {
                new StringValueType("b"),
                new StringValueType(""),
                new NumberValueType(150)
            },
            new BaseValueType[]
            {
                new StringValueType("file1.txt"),
                new StringValueType(".txt"),
                new NumberValueType(178)
            },
            new BaseValueType[]
            {
                new StringValueType("file2.jpg"),
                new StringValueType(".jpg"),
                new NumberValueType(91000)
            },
            new BaseValueType[]
            {
                new StringValueType("subDirectory"),
                new NullValueType(),
                new NumberValueType(300)
            }
        };

        var query = _parserFixture.Sut.Parse(givenInput);

        var actualResult = evaluation.Evaluate(query).Rows.ToList();

        actualResult.Should().BeEquivalentTo(expectedResult,
            o => o.ComparingRecordsByValue().WithStrictOrdering());
    }
}
