namespace Fsql.Core.FileSystem;

public enum FileSystemEntryType
{
    File, Directory
}

public abstract record BaseFileSystemEntry
{
    public string FullPath { get; }
    public FileSystemEntryType Type { get; }

    public abstract double Size { get; }
    public abstract string AbsolutePath { get; }
    public abstract DateTime AccessTime { get; }
    public abstract DateTime CreateTime { get; }
    public abstract DateTime ModifyTime { get; }

    protected BaseFileSystemEntry(string fullPath, FileSystemEntryType type)
    {
        FullPath = fullPath.TrimEnd('/', '\\');
        Type = type;
    }
}

public record FileSystemEntry : BaseFileSystemEntry
{
    public override double Size => Details.Length;
    public override string AbsolutePath => Details.FullName;
    public override DateTime AccessTime => Details.LastAccessTime;
    public override DateTime CreateTime => Details.CreationTime;
    public override DateTime ModifyTime => Details.LastWriteTime;

    public FileSystemEntry(string fullPath, FileSystemEntryType type)
        : base(fullPath, type) { }

    private FileInfo? _details;

    private FileInfo Details => _details ??= new FileInfo(FullPath);
}
