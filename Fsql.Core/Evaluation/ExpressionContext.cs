using Fsql.Core.Functions;

namespace Fsql.Core.Evaluation;

public class ExpressionContext : IExpressionContext
{
    private readonly IRow _row;
    private readonly IReadOnlyDictionary<Identifier, IFunction> _functions;

    public ExpressionContext(IRow row, IReadOnlyDictionary<Identifier, IFunction> functions)
    {
        _row = row;
        _functions = functions;
    }

    public BaseValueType Get(Identifier identifier) => _row.Get(identifier);

    public IFunction GetFunction(Identifier identifier) => _functions[identifier];
}
