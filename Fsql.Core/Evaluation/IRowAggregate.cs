namespace Fsql.Core.Evaluation;

public class AggregateAttributeException : ApplicationException
{
    public AggregateAttributeException(string message) : base(message) { }
}

public interface IRowAggregate
{
    BaseValueType GetAggregated(Identifier attribute);

    IEnumerable<BaseValueType> GetMany(Identifier attribute);

    Identifier AggregateKey { get; }

    int RowsCount { get; }
}

public class RowAggregate : IRowAggregate
{
    private readonly IReadOnlyList<IRow> _rows;

    public RowAggregate(IReadOnlyList<IRow> rows, Identifier aggregateKey)
    {
        _rows = rows;
        AggregateKey = aggregateKey;
    }

    public BaseValueType GetAggregated(Identifier attribute)
    {
        if (attribute != AggregateKey)
            throw new AggregateAttributeException($"'{attribute.Name}' is not an aggregate attribute.");

        return _rows[0].Get(attribute);
    }

    public IEnumerable<BaseValueType> GetMany(Identifier attribute)
    {
        return _rows.Select(row => row.Get(attribute));
    }

    public Identifier AggregateKey { get; }

    public int RowsCount => _rows.Count;
}
