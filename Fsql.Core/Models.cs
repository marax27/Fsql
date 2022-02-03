namespace Fsql.Core;

public record Query(
    IReadOnlyCollection<string> SelectedAttributes,
    string FromPath,
    OrderByExpression OrderByExpression
);

public sealed record OrderByExpression(IReadOnlyCollection<string> Attributes)
{
    public static OrderByExpression NoOrdering => new (new List<string>());

    public bool Equals(OrderByExpression? other) =>
        other is { } && Attributes.SequenceEqual(other.Attributes);

    public override int GetHashCode() => Attributes.GetHashCode();
}
