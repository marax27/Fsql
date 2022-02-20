using Fsql.Core.Evaluation;

namespace Fsql.Core;

public interface IExpressionContext
{
    BaseValueType Get(Identifier identifier);
    BaseValueType EvaluateFunction(Identifier identifier, IReadOnlyList<Expression> arguments);
}

public abstract record Expression
{
    public abstract BaseValueType Evaluate(IExpressionContext context);
}

public record NumberConstant(double Value) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context) => new NumberValueType(Value);
}

public record StringConstant(string Value) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context) => new StringValueType(Value);
}

public record NullConstant : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context) => new NullValueType();
}

public record FunctionCall(Identifier Identifier, IReadOnlyList<Expression> Arguments) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var functionResult = context.EvaluateFunction(Identifier, Arguments);
        return functionResult;
    }
}

public record IdentifierReferenceExpression(Identifier Identifier) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context) => context.Get(Identifier);
}

public abstract record BinaryOperatorExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);
        return left is NullValueType || right is NullValueType
            ? new NullValueType()
            : new BooleanValueType(EvaluateOperation(left, right));
    }

    protected abstract bool EvaluateOperation(BaseValueType left, BaseValueType right);
}

public record EqualsExpression(Expression Left, Expression Right) : BinaryOperatorExpression(Left, Right)
{
    protected override bool EvaluateOperation(BaseValueType left, BaseValueType right)
        => left.Equals(right);
}

public record NotEqualExpression(Expression Left, Expression Right) : BinaryOperatorExpression(Left, Right)
{
    protected override bool EvaluateOperation(BaseValueType left, BaseValueType right)
        => !left.Equals(right);
}

public record GreaterThanExpression(Expression Left, Expression Right) : BinaryOperatorExpression(Left, Right)
{
    protected override bool EvaluateOperation(BaseValueType left, BaseValueType right)
        => left.CompareTo(right) > 0;
}

public record LessThanExpression(Expression Left, Expression Right) : BinaryOperatorExpression(Left, Right)
{
    protected override bool EvaluateOperation(BaseValueType left, BaseValueType right)
        => left.CompareTo(right) < 0;
}

public record GreaterThanOrEqualExpression(Expression Left, Expression Right) : BinaryOperatorExpression(Left, Right)
{
    protected override bool EvaluateOperation(BaseValueType left, BaseValueType right)
        => left.CompareTo(right) >= 0;
}

public record LessThanOrEqualExpression(Expression Left, Expression Right) : BinaryOperatorExpression(Left, Right)
{
    protected override bool EvaluateOperation(BaseValueType left, BaseValueType right)
        => left.CompareTo(right) <= 0;
}

public record AndExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var left = CheckType(Left.Evaluate(context));
        if (left is false)
            return new BooleanValueType(false);

        var right = CheckType(Right.Evaluate(context));
        if (right is false)
            return new BooleanValueType(false);

        return (left is null || right is null) ? new NullValueType() : new BooleanValueType(true);
    }

    private bool? CheckType(BaseValueType value) => value switch
    {
        BooleanValueType boolValue => boolValue.Value,
        NullValueType _ => null,
        _ => throw new CastException($"AND operator expects a boolean (or null) argument: {value} received.")
    };
}

public record OrExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var left = CheckType(Left.Evaluate(context));
        if (left is true)
            return new BooleanValueType(true);

        var right = CheckType(Right.Evaluate(context));
        if (right is true)
            return new BooleanValueType(true);

        return (left is null || right is null) ? new NullValueType() : new BooleanValueType(false);
    }

    private bool? CheckType(BaseValueType value) => value switch
    {
        BooleanValueType boolValue => boolValue.Value,
        NullValueType _ => null,
        _ => throw new CastException($"OR operator expects a boolean (or null) argument: {value} received.")
    };
}
