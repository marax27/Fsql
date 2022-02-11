using Fsql.Core.FileSystem.Abstractions;

namespace Fsql.Core.FileSystem;

public record FileNodeEntry : BaseFileSystemEntry
{
    public override double Size => Details.Length;
    public override string AbsolutePath => Details.FullName;
    public override DateTime AccessTime => Details.LastAccessTime;
    public override DateTime CreateTime => Details.CreationTime;
    public override DateTime ModifyTime => Details.LastWriteTime;

    public FileNodeEntry(string fullPath)
        : base(fullPath, FileSystemEntryType.File) { }

    private FileInfo? _details;

    private FileInfo Details => _details ??= new FileInfo(FullPath);
}

public record DirectoryNodeEntry : BaseFileSystemEntry
{
    public override double Size => _size ??= CalculateSize();
    public override string AbsolutePath => Details.FullName;
    public override DateTime AccessTime => Details.LastAccessTime;
    public override DateTime CreateTime => Details.CreationTime;
    public override DateTime ModifyTime => Details.LastWriteTime;

    public DirectoryNodeEntry(string fullPath)
        : base(fullPath, FileSystemEntryType.Directory) { }

    private DirectoryInfo? _details;
    private double? _size;

    private DirectoryInfo Details => _details ??= new DirectoryInfo(FullPath);

    private double CalculateSize() => Details
        .EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
}
