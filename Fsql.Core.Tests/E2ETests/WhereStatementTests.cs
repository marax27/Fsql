using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem.Abstractions;
using Fsql.Core.Tests.WhenEvaluating;
using Fsql.Core.Tests.WhenParsing;
using Xunit;

namespace Fsql.Core.Tests.E2ETests;

public class WhereStatementTests : IClassFixture<ParserFixture>
{
    private readonly ParserFixture _parserFixture;

    public WhereStatementTests(ParserFixture parserFixture)
    {
        _parserFixture = parserFixture;
    }

    [Fact]
    public void Test1()
    {
        var givenInput = "SELECT name FROM /home WHERE extension = '.jpg' OR extension = '.pdf' ORDER BY name";

        var evaluation = new QueryEvaluation(FileSystemAccess);

        var expectedResult = new[]
        {
            new BaseValueType[] { new StringValueType("a2.jpg") },
            new BaseValueType[] { new StringValueType("a4.pdf") },
            new BaseValueType[] { new StringValueType("b2.jpg") },
            new BaseValueType[] { new StringValueType("b4.pdf") },
            new BaseValueType[] { new StringValueType("c2.jpg") },
            new BaseValueType[] { new StringValueType("c4.pdf") },
            new BaseValueType[] { new StringValueType("d2.jpg") },
            new BaseValueType[] { new StringValueType("d4.pdf") }
        };

        var query = _parserFixture.Sut.Parse(givenInput);

        var actualResult = evaluation.Evaluate(query).Rows.ToList();

        actualResult.Should().BeEquivalentTo(expectedResult,
            o => o.ComparingRecordsByValue().WithStrictOrdering());
    }

    [Fact]
    public void Test2()
    {
        var givenInput = "SELECT name FROM /home WHERE 1k < size AND size < 3k ORDER BY name";

        var evaluation = new QueryEvaluation(FileSystemAccess);

        var expectedResult = new[]
        {
            new BaseValueType[] { new StringValueType("b1.txt") },
            new BaseValueType[] { new StringValueType("b2.jpg") },
            new BaseValueType[] { new StringValueType("b3.mov") },
            new BaseValueType[] { new StringValueType("b4.pdf") },
            new BaseValueType[] { new StringValueType("c1.txt") },
            new BaseValueType[] { new StringValueType("c2.jpg") },
            new BaseValueType[] { new StringValueType("c3.mov") },
            new BaseValueType[] { new StringValueType("c4.pdf") }
        };

        var query = _parserFixture.Sut.Parse(givenInput);

        var actualResult = evaluation.Evaluate(query).Rows.ToList();

        actualResult.Should().BeEquivalentTo(expectedResult,
            o => o.ComparingRecordsByValue().WithStrictOrdering());
    }

    private IFileSystemAccess FileSystemAccess =>
        new FakeFileSystemAccess()
            .WithFiles("/home", 1000, "a1.txt", "a2.jpg", "a3.mov", "a4.pdf")
            .WithFiles("/home", 2000, "b1.txt", "b2.jpg", "b3.mov", "b4.pdf")
            .WithFiles("/home", 3000, "c1.txt", "c2.jpg", "c3.mov", "c4.pdf")
            .WithFiles("/home", 4000, "d1.txt", "d2.jpg", "d3.mov", "d4.pdf");
}
