namespace Fsql.Core.FileSystem
{
    public enum FileSystemEntryType
    {
        File, Directory
    }

    public record FileSystemEntry
    {
        public string FullPath { get; }
        public FileSystemEntryType Type { get; }

        public FileSystemEntry(string fullPath, FileSystemEntryType type)
        {
            FullPath = fullPath.TrimEnd('/', '\\');
            Type = type;
        }
    }

    public interface IFileSystemAccess
    {
        IEnumerable<FileSystemEntry> GetEntries(string directoryPath);
    }

    public class FileSystemAccess : IFileSystemAccess
    {
        public IEnumerable<FileSystemEntry> GetEntries(string directoryPath)
        {
            var files = Directory.EnumerateFiles(directoryPath)
                .Select(path => new FileSystemEntry(path, FileSystemEntryType.File));

            var directories = Directory.EnumerateDirectories(directoryPath)
                .Select(path => new FileSystemEntry(path, FileSystemEntryType.Directory));

            return files.Concat(directories);
        }
    }
}
