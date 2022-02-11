using Fsql.Core.FileSystem;
using Fsql.Core.FileSystem.Abstractions;

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
        var headers = expandedAttributes.Select(attribute => attribute.Name).ToList();
        var ordering = new EntryOrdering(query.OrderByExpression, queryContext);

        var allRows = GetFiltered(query, queryContext);
        var orderedRows = ordering.OrderBy(allRows);
        var rows = orderedRows
            .Select(e => CreateRow(e, expandedAttributes, queryContext))
            .ToList();

        return new(headers, rows);
    }

    private IEnumerable<BaseFileSystemEntry> GetFiltered(Query query, IQueryContext<BaseFileSystemEntry> context)
    {
        var allEntries = GetAllEntries(query.FromExpression);
        if (query.WhereExpression is null)
            return allEntries;
        else
        {
            var filtering = new EntryFiltering(query.WhereExpression, context);
            return filtering.Filter(allEntries);
        }
    }

    private IEnumerable<BaseFileSystemEntry> GetAllEntries(FromExpression fromExpression)
    {
        if (!fromExpression.Recursive)
        {
            foreach (var entry in _fileSystemAccess.GetEntries(fromExpression.Path))
                yield return entry;
            yield break;
        }

        var pathsToProcess = new Queue<string>();
        pathsToProcess.Enqueue(fromExpression.Path);

        while (pathsToProcess.Any())
        {
            var path = pathsToProcess.Dequeue();
            foreach (var entry in _fileSystemAccess.GetEntries(path))
            {
                if (entry.Type == FileSystemEntryType.Directory)
                    pathsToProcess.Enqueue(entry.AbsolutePath);
                yield return entry;
            }
        }
    }

    private static BaseValueType[] CreateRow(BaseFileSystemEntry entry, IReadOnlyCollection<Identifier> attributes, IQueryContext<BaseFileSystemEntry> queryContext)
    {
        var result = attributes
            .Select(attribute => queryContext.Get(attribute, entry))
            .ToArray();
        return result;
    }

    private static IReadOnlyCollection<Identifier> ExpandAttributes(IEnumerable<Identifier> attributes, IQueryContext<BaseFileSystemEntry> queryContext)
    {
        var result = new List<Identifier>();
        foreach (var attribute in attributes)
        {
            if (attribute.Name.Equals("*"))
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

        BaseValueType Predicate(BaseFileSystemEntry entry, OrderCondition condition) =>
            _context.Get(condition.Attribute, entry);

        var condition = _orderByExpression.Conditions.First();
        return condition.Ascending
            ? entries.OrderBy(e => Predicate(e, condition))
            : entries.OrderByDescending(e => Predicate(e, condition));
    }
}

internal class EntryFiltering
{
    private readonly Expression _filterExpression;
    private readonly IQueryContext<BaseFileSystemEntry> _context;

    public EntryFiltering(Expression filterExpression, IQueryContext<BaseFileSystemEntry> context)
    {
        _filterExpression = filterExpression ?? throw new ArgumentNullException(nameof(filterExpression));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IEnumerable<BaseFileSystemEntry> Filter(IEnumerable<BaseFileSystemEntry> entries)
    {
        return entries.Where(EvaluatesToTrue);
    }

    private bool EvaluatesToTrue(BaseFileSystemEntry entry)
    {
        var expressionContext = new ExpressionContext(_context, entry);
        var result = _filterExpression.Evaluate(expressionContext);
        return result.EvaluatesToTrue();
    }
}

internal class ExpressionContext : IExpressionContext
{
    private readonly IQueryContext<BaseFileSystemEntry> _queryContext;
    private readonly BaseFileSystemEntry _entry;

    public ExpressionContext(IQueryContext<BaseFileSystemEntry> queryContext, BaseFileSystemEntry entry)
    {
        _queryContext = queryContext ?? throw new ArgumentNullException(nameof(queryContext));
        _entry = entry ?? throw new ArgumentNullException(nameof(entry));
    }

    public BaseValueType Get(Identifier identifier)
    {
        return _queryContext.Get(identifier, _entry);
    }
}
