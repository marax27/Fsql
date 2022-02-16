using Fsql.Core.Evaluation;

namespace Fsql.Core.Functions;

public interface IFunction
{
    BaseValueType Evaluate(IReadOnlyList<BaseValueType> arguments);
}

public abstract class BaseFunction : IFunction
{
    public abstract BaseValueType Evaluate(IReadOnlyList<BaseValueType> arguments);

    protected void AssertArgumentCount(int expectedCount, IReadOnlyList<BaseValueType> arguments)
    {
        if (arguments.Count != expectedCount)
            throw new ArgumentCountException(expectedCount, arguments.Count);
    }

    protected T GetRequired<T>(BaseValueType argument) where T : BaseValueType
    {
        return argument as T
               ?? throw new ArgumentTypeException(typeof(T).Name, argument.GetType().Name);
    }
}

public abstract class BaseUnaryFunction<TArgument, TResult> : BaseFunction
    where TArgument : BaseValueType
    where TResult : BaseValueType
{
    public override BaseValueType Evaluate(IReadOnlyList<BaseValueType> arguments)
    {
        AssertArgumentCount(1, arguments);
        var argument = GetRequired<TArgument>(arguments[0]);
        return EvaluateUnary(argument);
    }

    protected abstract TResult EvaluateUnary(TArgument argument);
}
