using Fsql.Core.Evaluation;

namespace Fsql.Core.Functions;

public interface IAggregateFunction
{
    BaseValueType Evaluate(IReadOnlyList<IEnumerable<BaseValueType>> arguments);
}

public class CountAggregateFunction : IAggregateFunction
{
    public BaseValueType Evaluate(IReadOnlyList<IEnumerable<BaseValueType>> arguments)
    {
        if (arguments.Count != 1)
            throw new ArgumentCountException(1, arguments.Count);
        var argument = arguments[0];

        return new NumberValueType(EvaluateInternal(argument));
    }

    private static int EvaluateInternal(IEnumerable<BaseValueType> argument) => argument.Count();
}
