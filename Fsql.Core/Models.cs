namespace Fsql.Core;

public record Query(
    IReadOnlyCollection<Expression> SelectedAttributes,
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

public sealed record OrderCondition(Expression Expression, bool Ascending);

public sealed record Identifier(string Name)
{
    public string Id => Name.ToLowerInvariant();

    public bool Equals(Identifier? other) =>
        other != null && Id.Equals(other.Id);

    public override int GetHashCode() =>
        Id.GetHashCode();

    public static Identifier Wildcard => new("*");
}
