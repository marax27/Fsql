using Fsql.Core.FileSystem;

namespace Fsql.Core
{
    public record QueryEvaluationResult(
        IReadOnlyCollection<string> AttributeNames,
        IReadOnlyCollection<BaseValueType[]> Rows);

    public class QueryEvaluation
    {
        private readonly IFileSystemAccess _fileSystemAccess;

        public QueryEvaluation(IFileSystemAccess fileSystemAccess)
        {
            _fileSystemAccess = fileSystemAccess ?? throw new ArgumentNullException(nameof(fileSystemAccess));
        }

        public QueryEvaluationResult Evaluate(Query query)
        {
            var entries = _fileSystemAccess.GetEntries(query.FromPath);

            var rows = entries.Select(CreateRow).ToList();
            var headers = new[] { "filename", "extension", "path", "type" };

            return new(headers, rows);
        }

        private static StringValueType[] CreateRow(FileSystemEntry entry)
        {
            return new StringValueType[]
            {
                new(Path.GetFileName(entry.FullPath)),
                new(Path.GetExtension(entry.FullPath)),
                new(entry.FullPath),
                new(entry.Type.ToString())
            };
        }
    }
}
