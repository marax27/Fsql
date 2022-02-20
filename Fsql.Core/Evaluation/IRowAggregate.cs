namespace Fsql.Core.Evaluation;

public class AggregateAttributeException : ApplicationException
{
    public AggregateAttributeException(string message) : base(message) { }
}

public interface IRowAggregate
{
    BaseValueType GetAggregated(Expression attribute);

    IEnumerable<BaseValueType> GetMany(Identifier attribute);

    Expression AggregateKey { get; }

    BaseValueType AggregateValue { get; }
}

public class RowAggregate : IRowAggregate
{
    private readonly IReadOnlyList<SingleRowExpressionContext> _rows;

    public RowAggregate(IReadOnlyList<SingleRowExpressionContext> rows, Expression aggregateKey, BaseValueType aggregateValue)
    {
        _rows = rows;
        AggregateKey = aggregateKey;
        AggregateValue = aggregateValue;
    }

    public BaseValueType GetAggregated(Expression attribute)
    {
        if (attribute != AggregateKey)
            throw new AggregateAttributeException($"'{attribute}' is not an aggregate attribute.");

        return AggregateValue;
    }

    public IEnumerable<BaseValueType> GetMany(Identifier attribute)
    {
        return _rows.Select(row => row.Get(attribute));
    }

    public Expression AggregateKey { get; }

    public BaseValueType AggregateValue { get; }
}
