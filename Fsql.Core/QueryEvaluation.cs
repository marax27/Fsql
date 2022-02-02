using Fsql.Core.FileSystem;

namespace Fsql.Core
{
    public interface IQueryContext<in TEntry>
    {
        public IReadOnlyCollection<string> Attributes { get; }
        
        public BaseValueType Get(string attribute, TEntry entry);
    }

    public class FileSystemQueryContext : IQueryContext<FileSystemEntry>
    {
        private const string NameAttribute = "name";
        private const string ExtensionAttribute = "extension";
        private const string TypeAttribute = "type";

        public IReadOnlyCollection<string> Attributes => new[]
        {
            NameAttribute, ExtensionAttribute, TypeAttribute
        };

        public BaseValueType Get(string attribute, FileSystemEntry entry)
        {
            return attribute.ToLower() switch
            {
                NameAttribute => new StringValueType(Path.GetFileName(entry.FullPath)),
                ExtensionAttribute => GetExtension(entry),
                TypeAttribute => new StringValueType(entry.Type.ToString()),
                _ => throw new ApplicationException($"Unknown attribute: {attribute}.")
            };
        }

        private static BaseValueType GetExtension(FileSystemEntry entry)
        {
            return entry.Type switch
            {
                FileSystemEntryType.File => new StringValueType(Path.GetExtension(entry.FullPath)),
                FileSystemEntryType.Directory => new NullValueType(),
                _ => throw new ApplicationException($"Unsupported filesystem entry type: {entry.Type}.")
            };
        }
    }

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
            var queryContext = new FileSystemQueryContext();
            var expandedAttributes = ExpandAttributes(query.SelectedAttributes, queryContext);
            var headers = expandedAttributes;
            
            var rows = _fileSystemAccess.GetEntries(query.FromPath)
                .Select(e => CreateRow(e, expandedAttributes, queryContext))
                .ToList();

            return new(headers, rows);
        }

        private static BaseValueType[] CreateRow(FileSystemEntry entry, IReadOnlyCollection<string> attributes, IQueryContext<FileSystemEntry> queryContext)
        {
            var result = attributes
                .Select(attribute => queryContext.Get(attribute, entry))
                .ToArray();
            return result;
        }

        private static IReadOnlyCollection<string> ExpandAttributes(IEnumerable<string> attributes, IQueryContext<FileSystemEntry> queryContext)
        {
            var result = new List<string>();
            foreach (var attribute in attributes)
            {
                if (attribute.Equals("*"))
                    result.AddRange(queryContext.Attributes);
                else
                    result.Add(attribute);
            }
            return result;
        }
    }
}
