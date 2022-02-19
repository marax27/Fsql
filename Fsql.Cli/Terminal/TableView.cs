using Fsql.Cli.Models;
using Fsql.Cli.Settings;

namespace Fsql.Cli.Terminal;

public class TableView
{
    private const string Separator = "|";

    private readonly IColumnWidthStrategy _columnWidthStrategy;

    public TableView(IColumnWidthStrategy columnWidthStrategy)
    {
        _columnWidthStrategy = columnWidthStrategy;
    }

    public IEnumerable<string> Render(TableModel table)
    {
        var columnWidths = _columnWidthStrategy.ComputeColumnWidths(table);
        var actualWidth = columnWidths.Sum() + Separator.Length * (table.ColumnCount - 1);

        yield return FormatRow(table.Headers);
        yield return new string('-', actualWidth);

        foreach (var row in table.Rows)
        {
            yield return FormatRow(row);
        }

        string FormatRow(IEnumerable<string> values)
        {
            var textsWithWidths = values.Zip(columnWidths);
            return string.Join(Separator, textsWithWidths.Select(cell => cell.First.PadRight(cell.Second)));
        }
    }
}
