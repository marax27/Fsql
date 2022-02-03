using Fsql.Core.FileSystem;

namespace Fsql.Core.Evaluation;

public record QueryEvaluationResult(
    IReadOnlyCollection<string> AttributeNames,
    IReadOnlyCollection<BaseValueType[]> Rows);

public class QueryEvaluation
{
    private readonly IFileSystemAccess _fileSystemAccess;

    public QueryEvaluation(IFileSystemAccess fileSystemAccess)
    {
        _fileSystemAccess = fileSystemAccess ?? throw new ArgumentNullException(nameof(fileSystemAccess));
    }

    public QueryEvaluationResult Evaluate(Query query)
    {
        var queryContext = new FileSystemQueryContext();
        var expandedAttributes = ExpandAttributes(query.SelectedAttributes, queryContext);
        var headers = expandedAttributes;
        var ordering = new EntryOrdering(query.OrderByExpression, queryContext);

        var allRows = _fileSystemAccess.GetEntries(query.FromPath);
        var orderedRows = ordering.OrderBy(allRows);
        var rows = orderedRows
            .Select(e => CreateRow(e, expandedAttributes, queryContext))
            .ToList();

        return new(headers, rows);
    }

    private static BaseValueType[] CreateRow(BaseFileSystemEntry entry, IReadOnlyCollection<string> attributes, IQueryContext<BaseFileSystemEntry> queryContext)
    {
        var result = attributes
            .Select(attribute => queryContext.Get(attribute, entry))
            .ToArray();
        return result;
    }

    private static IReadOnlyCollection<string> ExpandAttributes(IEnumerable<string> attributes, IQueryContext<BaseFileSystemEntry> queryContext)
    {
        var result = new List<string>();
        foreach (var attribute in attributes)
        {
            if (attribute.Equals("*"))
                result.AddRange(queryContext.Attributes);
            else
                result.Add(attribute);
        }
        return result;
    }
}

internal class EntryOrdering
{
    private readonly OrderByExpression _orderByExpression;
    private readonly IQueryContext<BaseFileSystemEntry> _context;

    public EntryOrdering(OrderByExpression orderByExpression, IQueryContext<BaseFileSystemEntry> context)
    {
        _orderByExpression = orderByExpression ?? throw new ArgumentNullException(nameof(orderByExpression));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IEnumerable<BaseFileSystemEntry> OrderBy(IEnumerable<BaseFileSystemEntry> entries)
    {
        if (_orderByExpression == OrderByExpression.NoOrdering)
            return entries;

        var orderByAttribute = _orderByExpression.Attributes.First();
        return entries
            .OrderBy(entry => _context.Get(orderByAttribute, entry).ToText());
    }
}
