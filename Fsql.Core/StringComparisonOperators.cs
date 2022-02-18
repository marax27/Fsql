using System.Text.RegularExpressions;
using Fsql.Core.Evaluation;

namespace Fsql.Core;

public abstract record BaseStringComparisonExpression : Expression
{
    protected bool? Matches(BaseValueType input, BaseValueType pattern)
    {
        var inputValue = ProcessArgument(input, nameof(input));
        var patternValue = ProcessArgument(pattern, nameof(pattern));

        if (inputValue is null || patternValue is null)
            return null;

        var escapedPattern = Regex.Escape(patternValue);
        var regexPattern = escapedPattern
            .Replace("%", ".*")
            .Replace("_", ".");

        return Regex.IsMatch(inputValue, $"^{regexPattern}$");
    }

    private string? ProcessArgument(BaseValueType argument, string argumentName) => argument switch
    {
        StringValueType stringArgument => stringArgument.Value,
        NullValueType => null,
        _ => throw new CastException(
            $"LIKE: invalid {argumentName} type: expected <{DataTypes.String}> or <{DataTypes.Null}>, received <{argument.Type}>.")
    };
}

public record LikeOperatorExpression(Expression Input, Expression Pattern) : BaseStringComparisonExpression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var inputValue = Input.Evaluate(context);
        var patternValue = Pattern.Evaluate(context);
        var match = Matches(inputValue, patternValue);
        
        return match is null
            ? new NullValueType()
            : new BooleanValueType(match.Value);
    }
}

public record NotLikeOperatorExpression(Expression Input, Expression Pattern) : BaseStringComparisonExpression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var inputValue = Input.Evaluate(context);
        var patternValue = Pattern.Evaluate(context);
        var match = Matches(inputValue, patternValue);

        return match is null
            ? new NullValueType()
            : new BooleanValueType(!match.Value);
    }
}
