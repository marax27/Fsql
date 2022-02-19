using System.Collections.Generic;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem.Abstractions;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluating;

public class WhenEvaluatingWithWhere
{
    [Fact]
    public void GivenFilteringByNameReturn1Row()
    {
        var fsAccess = new StubFileSystemAccess(GivenEntries);
        var nameId = new Identifier("name");
        var whereExpression = new EqualsExpression(
            new IdentifierReferenceExpression(nameId),
            new StringConstant("example.jpg")
        );
        var sut = new QueryEvaluation(new StubFileSystemAccess(GivenEntries));

        var givenQuery = new Query(new[] { new IdentifierReferenceExpression(nameId) }, new("./path", false), whereExpression, OrderByExpression.NoOrdering);

        var result = sut.Evaluate(givenQuery);
        result.Rows.Should().HaveCount(1);
    }

    private static string GivenPath => "./path";

    private static IEnumerable<BaseFileSystemEntry> GivenEntries => new List<BaseFileSystemEntry>
    {
        new FakeFileSystemEntry($"{GivenPath}/01.txt", FileSystemEntryType.File, 500),
        new FakeFileSystemEntry($"{GivenPath}/02.doc", FileSystemEntryType.File, 128),
        new FakeFileSystemEntry($"{GivenPath}/abc.jpg", FileSystemEntryType.File, 128),
        new FakeFileSystemEntry($"{GivenPath}/example.jpg", FileSystemEntryType.File, 1024),
        new FakeFileSystemEntry($"{GivenPath}/example.png", FileSystemEntryType.File, 2048),
        new FakeFileSystemEntry($"{GivenPath}/example.mov", FileSystemEntryType.File, 5555),
    };
}
