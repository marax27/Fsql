namespace Fsql.Core;

public record Query(
    IReadOnlyCollection<Identifier> SelectedAttributes,
    FromExpression FromExpression,
    Expression? WhereExpression,
    OrderByExpression OrderByExpression
);

public sealed record FromExpression(string Path, bool Recursive);

public sealed record OrderByExpression(IReadOnlyCollection<OrderCondition> Conditions)
{
    public static OrderByExpression NoOrdering => new (Array.Empty<OrderCondition>());

    public bool Equals(OrderByExpression? other) =>
        other is { } && Conditions.SequenceEqual(other.Conditions);

    public override int GetHashCode() => Conditions.GetHashCode();
}

public sealed record OrderCondition(Identifier Attribute, bool Ascending);

public sealed record Identifier(string Name);
