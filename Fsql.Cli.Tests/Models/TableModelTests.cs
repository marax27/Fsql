using System;
using System.Collections.Generic;
using FluentAssertions;
using Fsql.Cli.Models;
using Xunit;

namespace Fsql.Cli.Tests.Models;

public class TableModelTests
{
    [Fact]
    public void GivenSampleValuesContainExpectedRows()
    {
        var actualResult = new TableModel(_sampleHeaders, _sampleRows);

        actualResult.Rows.Should().BeEquivalentTo(_sampleRows,
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenSampleValuesContainExpectedHeaders()
    {
        var actualResult = new TableModel(_sampleHeaders, _sampleRows);

        actualResult.Headers.Should().BeEquivalentTo(_sampleHeaders,
            o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenEmptyHeadersThrowExpectedException()
    {
        var givenHeaders = Array.Empty<string>();

        var act = () =>
        {
            var actualResult = new TableModel(givenHeaders, _sampleRows);
        };

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("headers");
    }

    [Fact]
    public void GivenEmptyRowsThrowExpectedException()
    {
        var givenRows = Array.Empty<string[]>();

        var act = () =>
        {
            var actualResult = new TableModel(_sampleHeaders, givenRows);
        };

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("rows");
    }

    [Fact]
    public void Given1RowHasDifferentWidthThrowExpectedArgument()
    {
        var givenHeaders = new[] { "first", "second" };
        var givenRows = new[]
        {
            new[] { "a", "b" },
            new[] { "a", "b" },
            new[] { "a", "b" },
            new[] { "a", "b", "c" },
            new[] { "a", "b" },
        };

        var act = () =>
        {
            var actualResult = new TableModel(givenHeaders, givenRows);
        };

        act.Should().Throw<ArgumentException>();
    }

    private readonly string[] _sampleHeaders = { "name", "age" };

    private readonly string[][] _sampleRows = {
        new []{ "john", "30" },
        new []{ "kate", "27" },
        new []{ "bill", "19" },
    };
}
