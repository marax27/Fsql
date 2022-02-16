﻿using Fsql.Core.FileSystem.Abstractions;
using Fsql.Core.Functions;

namespace Fsql.Core.Evaluation;

public record QueryEvaluationResult(
    IReadOnlyCollection<string> AttributeNames,
    IReadOnlyCollection<BaseValueType[]> Rows);

public class QueryEvaluation
{
    private readonly IFileSystemAccess _fileSystemAccess;
    private readonly IReadOnlyDictionary<Identifier, IFunction> _functions;

    public QueryEvaluation(IFileSystemAccess fileSystemAccess)
    {
        _fileSystemAccess = fileSystemAccess ?? throw new ArgumentNullException(nameof(fileSystemAccess));
        _functions = new StringFunctionsModule().Load();
    }

    public QueryEvaluationResult Evaluate(Query query)
    {
        var queryContext = new FileSystemQueryContext();
        var expandedAttributes = ExpandAttributes(query.SelectedAttributes, queryContext.Attributes).ToList();

        var allRows = ComputeFrom(query.FromExpression, queryContext);
        var filteredRows = ComputeWhere(allRows, query.WhereExpression);
        var orderedRows = ComputeOrderBy(filteredRows, query.OrderByExpression);
        var selectResult = ComputeSelect(orderedRows, expandedAttributes).ToList();

        var attributeNames = expandedAttributes.Select(attribute => attribute.Name).ToList();
        return new(attributeNames, selectResult);
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
            var context = new ExpressionContext(row, _functions);
            var result = whereExpression.Evaluate(context);
            return result.EvaluatesToTrue();
        });
    }

    private IEnumerable<IRow> ComputeOrderBy(IEnumerable<IRow> rows, OrderByExpression expression)
    {
        if (expression == OrderByExpression.NoOrdering)
            return rows;

        BaseValueType Predicate(IRow row, OrderCondition condition)
        {
            var context = new ExpressionContext(row, _functions);
            return condition.Expression.Evaluate(context);
        }

        var condition = expression.Conditions.Single();
        return condition.Ascending
            ? rows.OrderBy(row => Predicate(row, condition))
            : rows.OrderByDescending(row => Predicate(row, condition));
    }

    private IEnumerable<BaseValueType[]> ComputeSelect(IEnumerable<IRow> rows, IReadOnlyCollection<Identifier> attributes)
    {
        return rows.Select(row => attributes.Select(row.Get).ToArray());
    }

    private IReadOnlyCollection<Identifier> ExpandAttributes(
        IEnumerable<Identifier> attributes, IReadOnlyCollection<Identifier> wildcardAttributes)
    {
        var result = new List<Identifier>();
        foreach (var attribute in attributes)
        {
            if (attribute.Name.Equals("*"))
                result.AddRange(wildcardAttributes);
            else
                result.Add(attribute);
        }

        return result;
    }
}
