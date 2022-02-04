namespace Fsql.Core;

public record Query(
    IReadOnlyCollection<string> SelectedAttributes,
    string FromPath,
    OrderByExpression OrderByExpression
);

public sealed record OrderByExpression(IReadOnlyCollection<OrderCondition> Conditions)
{
    public static OrderByExpression NoOrdering => new (new List<OrderCondition>());

    public bool Equals(OrderByExpression? other) =>
        other is { } && Conditions.SequenceEqual(other.Conditions);

    public override int GetHashCode() => Conditions.GetHashCode();
}

public sealed record OrderCondition(string Attribute, bool Ascending);
