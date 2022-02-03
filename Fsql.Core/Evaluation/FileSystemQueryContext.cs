using Fsql.Core.FileSystem;

namespace Fsql.Core.Evaluation;

public class FileSystemQueryContext : IQueryContext<BaseFileSystemEntry>
{
    private const string NameAttribute = "name";
    private const string ExtensionAttribute = "extension";
    private const string TypeAttribute = "type";
    private const string SizeAttribute = "size";
    private const string AccessTimeAttribute = "access_time";
    private const string CreateTimeAttribute = "create_time";
    private const string ModifyTimeAttribute = "modify_time";
    private const string AbsolutePathAttribute = "absolute_path";

    public IReadOnlyCollection<string> Attributes => new[]
    {
        NameAttribute, ExtensionAttribute, TypeAttribute, SizeAttribute,
        AccessTimeAttribute, CreateTimeAttribute, ModifyTimeAttribute,
        AbsolutePathAttribute
    };

    public BaseValueType Get(string attribute, BaseFileSystemEntry entry)
    {
        return attribute.ToLower() switch
        {
            NameAttribute => new StringValueType(Path.GetFileName(entry.FullPath)),
            ExtensionAttribute => GetExtension(entry),
            TypeAttribute => new StringValueType(entry.Type.ToString()),
            SizeAttribute => GetSize(entry),
            AbsolutePathAttribute => new StringValueType(entry.AbsolutePath),
            AccessTimeAttribute => new DateTimeValueType(entry.AccessTime),
            CreateTimeAttribute => new DateTimeValueType(entry.CreateTime),
            ModifyTimeAttribute => new DateTimeValueType(entry.ModifyTime),
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