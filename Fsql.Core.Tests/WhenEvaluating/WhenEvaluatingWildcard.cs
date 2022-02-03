﻿using FluentAssertions;
using Fsql.Core.Evaluation;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluating;

public class WhenEvaluatingWildcard
{
    [Fact]
    public void Given1WildcardAttributeReturnExpectedHeaders()
    {
        var fsAccess = new FakeFileSystemAccess()
            .WithFiles("./path", "document.txt", "code.cs");
        var query = new Query(new []{ "*" }, "./path", OrderByExpression.NoOrdering);
        var sut = new QueryEvaluation(fsAccess);

        var result = sut.Evaluate(query);

        result.AttributeNames.Should().BeEquivalentTo(
            new[] { "name", "extension", "type", "size", "access_time", "create_time", "modify_time", "absolute_path" },
            o => o.WithStrictOrdering());
    }
}