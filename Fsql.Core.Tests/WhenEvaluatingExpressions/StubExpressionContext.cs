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
}