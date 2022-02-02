using Fsql.Core.FileSystem;

namespace Fsql.Core.Evaluation;

public class FileSystemQueryContext : IQueryContext<BaseFileSystemEntry>
{
    private const string NameAttribute = "name";
    private const string ExtensionAttribute = "extension";
    private const string TypeAttribute = "type";
    private const string SizeAttribute = "size";

    public IReadOnlyCollection<string> Attributes => new[]
    {
        NameAttribute, ExtensionAttribute, TypeAttribute, SizeAttribute
    };

    public BaseValueType Get(string attribute, BaseFileSystemEntry entry)
    {
        return attribute.ToLower() switch
        {
            NameAttribute => new StringValueType(Path.GetFileName(entry.FullPath)),
            ExtensionAttribute => GetExtension(entry),
            TypeAttribute => new StringValueType(entry.Type.ToString()),
            SizeAttribute => GetSize(entry),
            _ => throw new ApplicationException($"Unknown attribute: {attribute}.")
        };
    }

    private static BaseValueType GetSize(BaseFileSystemEntry entry)
    {
        return entry.Type switch
        {
            FileSystemEntryType.File => new NumberValueType(entry.Size),
            FileSystemEntryType.Directory => new NullValueType(),
            _ => throw new ApplicationException($"Unsupported filesystem entry type: {entry.Type}.")
        };
    }

    private static BaseValueType GetExtension(BaseFileSystemEntry entry)
    {
        return entry.Type switch
        {
            FileSystemEntryType.File => new StringValueType(Path.GetExtension(entry.FullPath)),
            FileSystemEntryType.Directory => new NullValueType(),
            _ => throw new ApplicationException($"Unsupported filesystem entry type: {entry.Type}.")
        };
    }
}