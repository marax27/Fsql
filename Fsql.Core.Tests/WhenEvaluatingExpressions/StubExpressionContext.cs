using System;
using System.Collections.Generic;
using Fsql.Core.Evaluation;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions;

internal class StubExpressionContext : IExpressionContext
{
    public IReadOnlyDictionary<Identifier, BaseValueType> Values { get; }

    public StubExpressionContext(IReadOnlyDictionary<Identifier, BaseValueType> values)
    {
        Values = values;
    }

    public BaseValueType Get(Identifier identifier) => Values[identifier];

    public BaseValueType EvaluateFunction(Identifier identifier, IReadOnlyList<Expression> arguments)
    {
        throw new NotImplementedException();
    }

    public BaseValueType? TryGetCached(Expression expression) => null;
}