using Fsql.Core.Evaluation;

namespace Fsql.Core;

public interface IExpressionContext
{
    BaseValueType Get(Identifier identifier);
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

public record IdentifierReferenceExpression(Identifier Identifier) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context) => context.Get(Identifier);
}

public record EqualsExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var result = Left.Evaluate(context).Equals(Right.Evaluate(context));
        return new BooleanValueType(result);
    }
}

public record NotEqualExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var result = Left.Evaluate(context).Equals(Right.Evaluate(context));
        return new BooleanValueType(!result);
    }
}

public record GreaterThanExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var result = Left.Evaluate(context).CompareTo(Right.Evaluate(context));
        return new BooleanValueType(result > 0);
    }
}

public record LessThanExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var result = Left.Evaluate(context).CompareTo(Right.Evaluate(context));
        return new BooleanValueType(result < 0);
    }
}

public record GreaterThanOrEqualExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var result = Left.Evaluate(context).CompareTo(Right.Evaluate(context));
        return new BooleanValueType(result >= 0);
    }
}

public record LessThanOrEqualExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        var result = Left.Evaluate(context).CompareTo(Right.Evaluate(context));
        return new BooleanValueType(result <= 0);
    }
}

public record AndExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        // Hopefully, this will sometimes improve performance by skipping the right expression.
        if (!Left.Evaluate(context).EvaluatesToTrue())
            return new BooleanValueType(false);

        return new BooleanValueType(Right.Evaluate(context).EvaluatesToTrue());
    }
}

public record OrExpression(Expression Left, Expression Right) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context)
    {
        if (Left.Evaluate(context).EvaluatesToTrue())
            return new BooleanValueType(true);

        return new BooleanValueType(Right.Evaluate(context).EvaluatesToTrue());
    }
}
