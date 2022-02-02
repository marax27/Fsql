namespace Fsql.Core.Evaluation;

public interface IQueryContext<in TEntry>
{
    public IReadOnlyCollection<string> Attributes { get; }
        
    public BaseValueType Get(string attribute, TEntry entry);
}