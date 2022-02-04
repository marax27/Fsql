﻿using Fsql.Core.FileSystem;

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

        var allRows = _fileSystemAccess.GetEntries(query.FromPath);
        var orderedRows = ordering.OrderBy(allRows);
        var rows = orderedRows
            .Select(e => CreateRow(e, expandedAttributes, queryContext))
            .ToList();

        return new(headers, rows);
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