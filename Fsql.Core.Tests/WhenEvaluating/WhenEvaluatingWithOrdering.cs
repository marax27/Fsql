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
    public void GivenOrderedByNameAscendingReturnInCorrectOrder()
    {
        var expectedNames = new[] { "aaa", "ADirectory", "azz", "BDirectory", "ZDirectory" };
        var givenQuery = new Query(new[] { new Identifier("name") }, "./path", null, new(new[] { new OrderCondition(new("name"), true) }));
        var sut = new QueryEvaluation(new StubFileSystemAccess(GivenEntries));

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenOrderedByNameDescendingReturnInCorrectOrder()
    {
        var expectedNames = new[] { "ZDirectory", "BDirectory", "azz", "ADirectory", "aaa" };
        var givenQuery = new Query(new[] { new Identifier("name") }, "./path", null, new(new[] { new OrderCondition(new("name"), false) }));
        var sut = new QueryEvaluation(new StubFileSystemAccess(GivenEntries));

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenOrderedBySizeAscendingReturnInCorrectOrder()
    {
        var expectedNames = new[] { "azz", "aaa", "ZDirectory", "BDirectory", "ADirectory" };
        var givenQuery = new Query(
            new[] { new Identifier("name"), new("size") },
            "./path",
            null,
            new(new[] { new OrderCondition(new("size"), true) })
        );
        var sut = new QueryEvaluation(new StubFileSystemAccess(GivenEntries));

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenOrderedBySizeDescendingReturnInCorrectOrder()
    {
        var expectedNames = new[] { "ADirectory", "BDirectory", "ZDirectory", "aaa", "azz" };
        var givenQuery = new Query(
            new[] { new Identifier("name"), new("size") },
            "./path",
            null,
            new(new[] { new OrderCondition(new("size"), false) })
        );
        var sut = new QueryEvaluation(new StubFileSystemAccess(GivenEntries));

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }

    private static string GivenPath => "./path";

    private static IEnumerable<BaseFileSystemEntry> GivenEntries => new List<BaseFileSystemEntry>
    {
        new FakeFileSystemEntry($"{GivenPath}/aaa", FileSystemEntryType.File, 500),
        new FakeFileSystemEntry($"{GivenPath}/azz", FileSystemEntryType.File, 128),
        new FakeFileSystemEntry($"{GivenPath}/ZDirectory", FileSystemEntryType.Directory, 0),
        new FakeFileSystemEntry($"{GivenPath}/BDirectory", FileSystemEntryType.Directory, 0),
        new FakeFileSystemEntry($"{GivenPath}/ADirectory", FileSystemEntryType.Directory, 0),
    };
}