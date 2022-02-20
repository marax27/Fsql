using Fsql.Core.Functions;

namespace Fsql.Core.Evaluation;

public class SingleRowExpressionContext : IExpressionContext
{
    private readonly IRow _row;
    private readonly IReadOnlyDictionary<Identifier, IFunction> _functions;

    public SingleRowExpressionContext(IRow row, IReadOnlyDictionary<Identifier, IFunction> functions)
    {
        _row = row;
        _functions = functions;
    }

    public BaseValueType Get(Identifier identifier) => _row.Get(identifier);

    public BaseValueType EvaluateFunction(Identifier identifier, IReadOnlyList<Expression> arguments)
    {
        var argumentValues = arguments.Select(arg => arg.Evaluate(this)).ToList();
        var result = _functions[identifier].Evaluate(argumentValues);
        return result;
    }

    public BaseValueType? TryGetCached(Expression expression) => null;
}

public class AggregateExpressionContext : IExpressionContext
{
    private readonly IRowAggregate _aggregate;
    private readonly IReadOnlyDictionary<Identifier, IFunction> _functions;
    private readonly IReadOnlyDictionary<Identifier, IAggregateFunction> _aggregateFunctions;
    private readonly IReadOnlyDictionary<Expression, BaseValueType> _expressionCache;

    public AggregateExpressionContext(IRowAggregate aggregate,
        IReadOnlyDictionary<Identifier, IFunction> functions,
        IReadOnlyDictionary<Identifier, IAggregateFunction> aggregateFunctions)
    {
        _aggregate = aggregate;
        _functions = functions;
        _aggregateFunctions = aggregateFunctions;
        _expressionCache = new Dictionary<Expression, BaseValueType>
        {
            { _aggregate.AggregateKey, _aggregate.AggregateValue }
        };
    }

    public BaseValueType Get(Identifier identifier) =>
        _aggregate.GetAggregated(new IdentifierReferenceExpression(identifier));

    public BaseValueType EvaluateFunction(Identifier identifier, IReadOnlyList<Expression> arguments)
    {
        if (_aggregateFunctions.ContainsKey(identifier))
        {
            var evaluatedArguments = arguments
                .Select(argument => (argument as IdentifierReferenceExpression
                                     ?? throw new ApplicationException()).Identifier)
                .Select(_aggregate.GetMany)
                .ToList();

            var result = _aggregateFunctions[identifier].Evaluate(evaluatedArguments);
            return result;
        }

        if (_functions.ContainsKey(identifier))
        {
            var evaluatedArguments = arguments
                .Select(argument => argument.Evaluate(this))
                .ToList();

            var result = _functions[identifier].Evaluate(evaluatedArguments);
            return result;
        }

        throw new ApplicationException($"Cannot evaluate '{identifier.Name}': function not found.");
    }

    public BaseValueType? TryGetCached(Expression expression)
    {
        var result = _expressionCache.ContainsKey(expression) ? _expressionCache[expression] : null;
        return result;
    }
}
