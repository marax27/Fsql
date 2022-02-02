using Fsql.Core.FileSystem;

namespace Fsql.Core.Evaluation;

public class FileSystemQueryContext : IQueryContext<FileSystemEntry>
{
    private const string NameAttribute = "name";
    private const string ExtensionAttribute = "extension";
    private const string TypeAttribute = "type";

    public IReadOnlyCollection<string> Attributes => new[]
    {
        NameAttribute, ExtensionAttribute, TypeAttribute
    };

    public BaseValueType Get(string attribute, FileSystemEntry entry)
    {
        return attribute.ToLower() switch
        {
            NameAttribute => new StringValueType(Path.GetFileName(entry.FullPath)),
            ExtensionAttribute => GetExtension(entry),
            TypeAttribute => new StringValueType(entry.Type.ToString()),
            _ => throw new ApplicationException($"Unknown attribute: {attribute}.")
        };
    }

    private static BaseValueType GetExtension(FileSystemEntry entry)
    {
        return entry.Type switch
        {
            FileSystemEntryType.File => new StringValueType(Path.GetExtension(entry.FullPath)),
            FileSystemEntryType.Directory => new NullValueType(),
            _ => throw new ApplicationException($"Unsupported filesystem entry type: {entry.Type}.")
        };
    }
}