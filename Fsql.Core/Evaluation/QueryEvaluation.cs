using Fsql.Core.FileSystem.Abstractions;
using Fsql.Core.Functions;

namespace Fsql.Core.Evaluation;

public record QueryEvaluationResult(
    IReadOnlyCollection<string> AttributeNames,
    IReadOnlyCollection<BaseValueType[]> Rows);

public class QueryEvaluation
{
    private readonly IFileSystemAccess _fileSystemAccess;
    private readonly IReadOnlyDictionary<Identifier, IFunction> _functions;
    private readonly IReadOnlyDictionary<Identifier, IAggregateFunction> _aggregateFunctions;

    public QueryEvaluation(IFileSystemAccess fileSystemAccess)
    {
        _fileSystemAccess = fileSystemAccess ?? throw new ArgumentNullException(nameof(fileSystemAccess));
        _functions = new StringFunctionsModule().Load();
        _aggregateFunctions = new Dictionary<Identifier, IAggregateFunction>
        {
            { new("count"), new CountAggregateFunction() }
        };
    }

    public QueryEvaluationResult Evaluate(Query query)
    {
        var queryContext = new FileSystemQueryContext();
        var expandedAttributes = ExpandAttributes(query.SelectedAttributes, queryContext.Attributes).ToList();
        var attributeNames = GetAttributeNames(expandedAttributes);

        var allRows = ComputeFrom(query.FromExpression, queryContext);
        var filteredRows = ComputeWhere(allRows, query.WhereExpression);

        var groupedContexts = ComputeGroupBy(CreateContexts(filteredRows), query.GroupByExpression);
        var orderedContexts = ComputeOrderBy(groupedContexts, query.OrderByExpression);
        var selectResult = ComputeSelect(orderedContexts, expandedAttributes).ToList();
        return new(attributeNames, selectResult);
    }

    private IReadOnlyCollection<string> GetAttributeNames(IEnumerable<Expression> expandedAttributes)
    {
        return expandedAttributes
            .Select(attribute => attribute switch
            {
                IdentifierReferenceExpression idReference => idReference.Identifier.Name,
                _ => "?"
            })
            .ToArray();
    }

    private IEnumerable<IRow> ComputeFrom(FromExpression fromExpression, IQueryContext<BaseFileSystemEntry> queryContext)
    {
        if (!fromExpression.Recursive)
        {
            foreach (var entry in _fileSystemAccess.GetEntries(fromExpression.Path))
                yield return new FileSystemEntryRow(queryContext, entry);
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
                yield return new FileSystemEntryRow(queryContext, entry);
            }
        }
    }

    private IEnumerable<IRow> ComputeWhere(IEnumerable<IRow> rows, Expression? whereExpression)
    {
        if (whereExpression is null)
            return rows;

        return rows.Where(row =>
        {
            var context = new SingleRowExpressionContext(row, _functions);
            var result = whereExpression.Evaluate(context);
            return result.EvaluatesToTrue();
        });
    }

    private IEnumerable<IExpressionContext> ComputeGroupBy(IEnumerable<SingleRowExpressionContext> rows, GroupByExpression expression)
    {
        if (expression == GroupByExpression.NoGrouping)
            return rows;

        var aggregateExpression = expression.Attributes.Single();

        var groups = rows
            .GroupBy(aggregateExpression.Evaluate)
            .Select(grouping => new RowAggregate(grouping.ToList(), aggregateExpression, grouping.Key));

        return CreateContexts(groups);
    }

    private IEnumerable<IExpressionContext> ComputeOrderBy(IEnumerable<IExpressionContext> contexts, OrderByExpression expression)
    {
        if (expression == OrderByExpression.NoOrdering)
            return contexts;

        var condition = expression.Conditions.Single();
        return condition.Ascending
            ? contexts.OrderBy(context => context.TryGetCached(condition.Expression) ?? condition.Expression.Evaluate(context))
            : contexts.OrderByDescending(condition.Expression.Evaluate);
    }

    private IEnumerable<BaseValueType[]> ComputeSelect(IEnumerable<IExpressionContext> contexts, IReadOnlyCollection<Expression> attributes)
    {
        foreach (var context in contexts)
        {
            yield return attributes
                .Select(attribute => context.TryGetCached(attribute) ?? attribute.Evaluate(context))
                .ToArray();
        }
    }

    private IEnumerable<SingleRowExpressionContext> CreateContexts(IEnumerable<IRow> rows) =>
        rows.Select(row => new SingleRowExpressionContext(row, _functions));

    private IEnumerable<IExpressionContext> CreateContexts(IEnumerable<IRowAggregate> aggregates) =>
        aggregates.Select(aggregate => new AggregateExpressionContext(aggregate, _functions, _aggregateFunctions));

    private IReadOnlyCollection<Expression> ExpandAttributes(
        IEnumerable<Expression> attributes, IReadOnlyCollection<Identifier> wildcardAttributes)
    {
        var result = new List<Expression>();
        foreach (var attribute in attributes)
        {
            if (attribute is IdentifierReferenceExpression idExpression && idExpression.Identifier == Identifier.Wildcard)
                result.AddRange(wildcardAttributes.Select(a => new IdentifierReferenceExpression(a)));
            else
                result.Add(attribute);
        }

        return result;
    }
}
