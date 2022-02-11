namespace Fsql.Core.FileSystem.Abstractions;

public interface IFileSystemAccess
{
    IEnumerable<BaseFileSystemEntry> GetEntries(string directoryPath);
}