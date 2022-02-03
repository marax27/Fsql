using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluating;

public class WhenEvaluatingWithOrdering
{
    [Fact]
    public void GivenFilesAndDirectoriesOrderedByNameReturnInCorrectOrder()
    {
        var expectedNames = new[] { "aaa", "ADirectory", "azz", "BDirectory", "ZDirectory" };
        var givenQuery = new Query(new[] { "name" }, "./path", new(new[] { "name" }));
        var sut = new QueryEvaluation(new StubFileSystemAccess());

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenFilesAndDirectoriesOrderedBySizeReturnInCorrectOrder()
    {
        var expectedNames = new[] { "azz", "aaa", "ZDirectory", "BDirectory", "ADirectory" };
        var givenQuery = new Query(new[] { "name", "size" }, "./path", new(new[] { "size" }));
        var sut = new QueryEvaluation(new StubFileSystemAccess());

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }
}

internal class StubFileSystemAccess : IFileSystemAccess
{
    public const string GivenPath = "./path";

    public IEnumerable<BaseFileSystemEntry> GetEntries(string directoryPath)
        => new List<BaseFileSystemEntry>
        {
            new FakeFileSystemEntry($"{GivenPath}/aaa", FileSystemEntryType.File, 500),
            new FakeFileSystemEntry($"{GivenPath}/azz", FileSystemEntryType.File, 128),
            new FakeFileSystemEntry($"{GivenPath}/ZDirectory", FileSystemEntryType.Directory, 0),
            new FakeFileSystemEntry($"{GivenPath}/BDirectory", FileSystemEntryType.Directory, 0),
            new FakeFileSystemEntry($"{GivenPath}/ADirectory", FileSystemEntryType.Directory, 0),
        };
}
