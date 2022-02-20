namespace Fsql.Core.Evaluation;

public interface IQueryContext<in TEntry>
{
    public IReadOnlyCollection<Identifier> Attributes { get; }
        
    public BaseValueType Get(Identifier attribute, TEntry entry);
}
