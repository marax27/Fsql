using Fsql.Core.FileSystem.Abstractions;

namespace Fsql.Core.Evaluation;

public interface IRow
{
    BaseValueType Get(Identifier attribute);
}

public class FileSystemEntryRow : IRow
{
    private readonly IQueryContext<BaseFileSystemEntry> _queryContext;
    private readonly BaseFileSystemEntry _entry;

    public FileSystemEntryRow(IQueryContext<BaseFileSystemEntry> queryContext, BaseFileSystemEntry entry)
    {
        _queryContext = queryContext;
        _entry = entry;
    }

    public BaseValueType Get(Identifier attribute) => _queryContext.Get(attribute, _entry);
}
