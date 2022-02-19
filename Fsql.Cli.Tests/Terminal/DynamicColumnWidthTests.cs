using FluentAssertions;
using Fsql.Cli.Models;
using Fsql.Cli.Settings;
using Xunit;

namespace Fsql.Cli.Tests.Terminal;

public class DynamicColumnWidthTests
{
    [Fact]
    public void GivenSimpleTableReturnExpectedWidths()
    {
        var givenTable = new TableModel(
            new []{"name", "age"},
            new []
            {
                new []{"john", "22"},
                new []{"bob", "9"},
                new []{"maxwell", "30"},
            }
        );

        var sut = new DynamicColumnWidthStrategy();

        var actualResult = sut.ComputeColumnWidths(givenTable);

        actualResult.Should().BeEquivalentTo(new[] {7, 3}, o => o.WithStrictOrdering());
    }

    [Fact]
    public void GivenSingleColumnWithLongValueReturnExpectedWidth()
    {
        var givenTable = new TableModel(
            new[] { "name" },
            new[]
            {
                new []{ "12345678901234567890" },
                new []{ "bob" },
                new []{ "maxwell" },
            }
        );

        var sut = new DynamicColumnWidthStrategy();

        var actualResult = sut.ComputeColumnWidths(givenTable);

        actualResult.Should().BeEquivalentTo(new[] { 20 }, o => o.WithStrictOrdering());
    }

    [Fact]
    public void Given2WideColumnsReturnExpectedWidths()
    {
        var givenTable = new TableModel(
            new[] { "name", "phone number" },
            new[]
            {
                new []{ "jane", "30000000" },
                new []{ "bob", "40000000" },
                new []{ "maxwell", "50000000" },
            }
        );

        var sut = new DynamicColumnWidthStrategy();

        var actualResult = sut.ComputeColumnWidths(givenTable);

        actualResult.Should().BeEquivalentTo(new[] { 7, 12 }, o => o.WithStrictOrdering());
    }
}
