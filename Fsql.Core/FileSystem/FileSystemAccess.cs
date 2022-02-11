using Fsql.Core.FileSystem.Abstractions;

namespace Fsql.Core.FileSystem;

public class FileSystemAccess : IFileSystemAccess
{
    public IEnumerable<BaseFileSystemEntry> GetEntries(string directoryPath)
    {
        var files = Directory.EnumerateFiles(directoryPath)
            .Select(path => new FileNodeEntry(path));

        var directories = Directory.EnumerateDirectories(directoryPath)
            .Select(path => new DirectoryNodeEntry(path));

        return files.Concat<BaseFileSystemEntry>(directories);
    }
}
