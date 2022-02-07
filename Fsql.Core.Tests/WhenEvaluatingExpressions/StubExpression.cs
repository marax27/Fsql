using Fsql.Core.Evaluation;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions;

internal record StubExpression(BaseValueType Value) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context) => Value;
}
