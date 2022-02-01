namespace Fsql.Core
{
    public record Query(
        IReadOnlyCollection<string> SelectedAttributes,
        string FromPath
    );
}
