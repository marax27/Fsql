using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluating;

public class WhenEvaluatingWithOrdering
{
    [Fact]
    public void GivenFilesAndDirectoriesOrderedByNameReturnInCorrectOrder()
    {
        var fsAccess = new FakeFileSystemAccess()
            .WithDirectories("./path", "ADirectory", "BDirectory", "ZDirectory")
            .WithFiles("./path", "aaa", "azz");
        var expectedNames = new[] { "aaa", "ADirectory", "azz", "BDirectory", "ZDirectory" };
        var givenQuery = new Query(
            new[] { "name" },
            "./path",
            new OrderByExpression(new[] { "name" })
        );
        var sut = new QueryEvaluation(fsAccess);

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }
}
