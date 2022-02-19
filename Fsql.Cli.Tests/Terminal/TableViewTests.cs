using FluentAssertions;
using Fsql.Cli.Models;
using Fsql.Cli.Settings;
using Fsql.Cli.Terminal;
using Xunit;

namespace Fsql.Cli.Tests.Terminal;

public class TableViewTests
{
    [Fact]
    public void GivenSampleTableReturnExpectedText()
    {
        var givenModel = new TableModel(
            new []{ "first", "other" },
            new[]
            {
                new[] { "123", "456" },
                new[] { "xyz", "abc" },
            }
        );
        var givenStrategy = new DynamicColumnWidthStrategy();

        var sut = new TableView(givenStrategy);

        var actualResult = sut.Render(givenModel);

        actualResult.Should().BeEquivalentTo(new[]
        {
            "first|other",
            "-----------",
            "123  |456  ",
            "xyz  |abc  "
        }, o => o.WithStrictOrdering());
    }
}
