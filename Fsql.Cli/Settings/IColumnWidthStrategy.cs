using Fsql.Cli.Models;

namespace Fsql.Cli.Settings;

public interface IColumnWidthStrategy
{
    int[] ComputeColumnWidths(TableModel table);
}

public class DynamicColumnWidthStrategy : IColumnWidthStrategy
{
    public int[] ComputeColumnWidths(TableModel table)
    {
        var columnCount = table.ColumnCount;
        var columnWidths = table.Headers.Select(header => header.Length).ToArray();

        foreach (var row in table.Rows)
        {
            for (var i = 0; i < columnCount; ++i)
                columnWidths[i] = Math.Max(columnWidths[i], row[i].Length);
        }

        return columnWidths;
    }
}
