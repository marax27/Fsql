namespace Fsql.Core.Functions;

public interface IFunctionsModule
{
    IReadOnlyDictionary<Identifier, IFunction> Load();
}
