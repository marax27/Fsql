using Fsql.Core.Evaluation;

namespace Fsql.Core.Functions;

public class StringFunctionsModule : IFunctionsModule
{
    public IReadOnlyDictionary<Identifier, IFunction> Load() => new Dictionary<Identifier, IFunction>
    {
        { new("human"), new HumanFunction() },
        { new("lower"), new LowerFunction() },
        { new("upper"), new UpperFunction() },
        { new("length"), new LengthFunction() },
        { new("trim"), new TrimFunction() },
        { new("concat"), new ConcatFunction() }
    };
}

public class HumanFunction : BaseUnaryFunction<NumberValueType, StringValueType>
{
    protected override StringValueType EvaluateUnary(NumberValueType argument)
    {
        var value = argument.Value;
        var kilo = 1024;
        var units = new Queue<string>(new[] { "", "k", "M", "G", "T" });

        while (units.Count > 0)
        {
            var unit = units.Dequeue();

            if (value < kilo)
            {
                var resultText = (value >= 10.0 || unit == "") ? $"{value}" : $"{value:0.0}";
                return new StringValueType(resultText + unit);
            }

            value /= kilo;
        }

        return new StringValueType("inf");
    }
}

public class LowerFunction : BaseUnaryFunction<StringValueType, StringValueType>
{
    protected override StringValueType EvaluateUnary(StringValueType argument)
    {
        return new StringValueType(argument.Value.ToLower());
    }
}

public class UpperFunction : BaseUnaryFunction<StringValueType, StringValueType>
{
    protected override StringValueType EvaluateUnary(StringValueType argument)
    {
        return new StringValueType(argument.Value.ToUpper());
    }
}

public class LengthFunction : BaseUnaryFunction<StringValueType, NumberValueType>
{
    protected override NumberValueType EvaluateUnary(StringValueType argument)
    {
        return new NumberValueType(argument.Value.Length);
    }
}

public class TrimFunction : BaseUnaryFunction<StringValueType, StringValueType>
{
    protected override StringValueType EvaluateUnary(StringValueType argument)
    {
        return new StringValueType(argument.Value.Trim());
    }
}

public class ConcatFunction : BaseFunction
{
    public override BaseValueType Evaluate(IReadOnlyList<BaseValueType> arguments)
    {
        var values = arguments
            .Select(argument => GetRequired<StringValueType>(argument).Value);
        return new StringValueType(string.Join("", values));
    }
}
