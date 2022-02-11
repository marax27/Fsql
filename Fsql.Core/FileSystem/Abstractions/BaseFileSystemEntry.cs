namespace Fsql.Core.FileSystem.Abstractions;

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
