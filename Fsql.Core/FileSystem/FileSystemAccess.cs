namespace Fsql.Core.FileSystem;

public interface IFileSystemAccess
{
    IEnumerable<BaseFileSystemEntry> GetEntries(string directoryPath);
}

public class FileSystemAccess : IFileSystemAccess
{
    public IEnumerable<BaseFileSystemEntry> GetEntries(string directoryPath)
    {
        var files = Directory.EnumerateFiles(directoryPath)
            .Select(path => new FileSystemEntry(path, FileSystemEntryType.File));

        var directories = Directory.EnumerateDirectories(directoryPath)
            .Select(path => new FileSystemEntry(path, FileSystemEntryType.Directory));

        return files.Concat(directories);
    }
}
