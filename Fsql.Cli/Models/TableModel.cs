namespace Fsql.Cli.Models;

public class TableModel
{
    public IReadOnlyList<string> Headers { get; }
    public IReadOnlyList<string[]> Rows { get; }

    public TableModel(IReadOnlyList<string> headers, IReadOnlyList<string[]> rows)
    {
        if (headers.Count == 0)
            throw new ArgumentException("No headers provided.", nameof(headers));
        if (rows.Any(row => row.Length != headers.Count))
            throw new ArgumentException("Not all rows have the same length as the header.");

        Headers = headers;
        Rows = rows;
    }

    public int ColumnCount => Headers.Count;
}
