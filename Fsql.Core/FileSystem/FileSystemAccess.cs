namespace Fsql.Core.FileSystem
{
    public enum FileSystemEntryType
    {
        File, Directory
    }

    public abstract record BaseFileSystemEntry
    {
        public string FullPath { get; }
        public FileSystemEntryType Type { get; }
        
        public abstract double Size { get; }

        protected BaseFileSystemEntry(string fullPath, FileSystemEntryType type)
        {
            FullPath = fullPath.TrimEnd('/', '\\');
            Type = type;
        }
    }

    public record FileSystemEntry : BaseFileSystemEntry
    {
        public override double Size => Details.Length;

        public FileSystemEntry(string fullPath, FileSystemEntryType type)
            : base(fullPath, type) {}

        private FileInfo? _details;

        private FileInfo Details => _details ??= new FileInfo(FullPath);
    }

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
}
