﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem.Abstractions;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluating;

public class WhenEvaluatingWithOrdering
{
    [Fact]
    public void GivenOrderedByNameAscendingReturnInCorrectOrder()
    {
        var expectedNames = new[] { "ADirectory", "BDirectory", "ZDirectory", "aaa", "azz" };
        var givenQuery = new Query(new[] { new IdentifierReferenceExpression(new("name")) },
            new("./path", false),
            null,
            GroupByExpression.NoGrouping,
            new(new[] { new OrderCondition(new IdentifierReferenceExpression(new("name")), true) }));
        var sut = new QueryEvaluation(new StubFileSystemAccess(GivenEntries));

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenOrderedByNameDescendingReturnInCorrectOrder()
    {
        var expectedNames = new[] { "azz", "aaa", "ZDirectory", "BDirectory", "ADirectory" };
        var givenQuery = new Query(new[] { new IdentifierReferenceExpression(new("name")) }, new("./path", false), null, GroupByExpression.NoGrouping, new(new[] { new OrderCondition(new IdentifierReferenceExpression(new("name")), false) }));
        var sut = new QueryEvaluation(new StubFileSystemAccess(GivenEntries));

        var result = sut.Evaluate(givenQuery);

        var actualNames = result.Rows.Select(row => row[0].ToText());
        actualNames.Should().BeEquivalentTo(expectedNames,
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenOrderedBySizeAscendingReturnInCorrectOrder()
    {
        var expectedNames = new[] { "ZDirectory", "BDirectory", "ADirectory", "azz", "aaa" };
        var givenQuery = new Query(
            new[] { new IdentifierReferenceExpression(new("name")), new IdentifierReferenceExpression(new("size")) },
            new("./path", false),
            null,
            GroupByExpression.NoGrouping,
            new(new[] { new OrderCondition(new IdentifierReferenceExpression(new("size")), true) })
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
        var expectedNames = new[] { "aaa", "azz", "ZDirectory", "BDirectory", "ADirectory" };
        var givenQuery = new Query(
            new[] { new IdentifierReferenceExpression(new("name")), new IdentifierReferenceExpression(new("size")) },
            new("./path", false),
            null,
            GroupByExpression.NoGrouping,
            new(new[] { new OrderCondition(new IdentifierReferenceExpression(new("size")), false) })
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