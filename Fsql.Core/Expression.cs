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

public record ConstantExpression(BaseValueType Value) : Expression
{
    public override BaseValueType Evaluate(IExpressionContext context) => Value;
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
