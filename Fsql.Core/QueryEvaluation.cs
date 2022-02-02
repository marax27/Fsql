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

            var expandedAttributes = ExpandAttributes(query.SelectedAttributes);

            var headers = CreateHeaders(expandedAttributes);
            var rows = entries
                .Select(e => CreateRow(e, expandedAttributes).ToArray())
                .ToList();

            return new(headers, rows);
        }

        private static List<BaseValueType> CreateRow(FileSystemEntry entry, IReadOnlyCollection<string> attributes)
        {
            var result = new List<BaseValueType>();
            foreach (var attribute in attributes)
            {
                ProcessAttribute(entry, attribute, result);
            }
            return result;
        }

        private static void ProcessAttribute(FileSystemEntry entry, string attributeName, List<BaseValueType> row)
        {
            var comparison = StringComparison.OrdinalIgnoreCase;

            if (attributeName.Equals("name", comparison))
                row.Add(ReadNameAttribute(entry));
            else if (attributeName.Equals("extension", comparison))
                row.Add(ReadExtensionAttribute(entry));
            else if (attributeName.Equals("type", comparison))
                row.Add(ReadTypeAttribute(entry));
            else
                row.Add(new NullValueType());
        }

        private static StringValueType ReadNameAttribute(FileSystemEntry entry)
            => new (Path.GetFileName(entry.FullPath));

        private static BaseValueType ReadExtensionAttribute(FileSystemEntry entry)
            => entry.Type switch
            {
                FileSystemEntryType.File => new StringValueType(Path.GetExtension(entry.FullPath)),
                FileSystemEntryType.Directory => new NullValueType(),
                _ => throw new ApplicationException($"Unsupported filesystem entry type: {entry.Type}.")
            };

        private static StringValueType ReadTypeAttribute(FileSystemEntry entry)
            => new (entry.Type.ToString());

        private static IReadOnlyCollection<string> CreateHeaders(IReadOnlyCollection<string> attributes)
        {
            return attributes;
        }

        private static IReadOnlyCollection<string> ExpandAttributes(IReadOnlyCollection<string> attributes)
        {
            var baseAttributes = new[] { "name", "extension", "type" };
            var result = new List<string>();
            foreach (var attribute in attributes)
            {
                if (attribute.Equals("*"))
                    result.AddRange(baseAttributes);
                else
                    result.Add(attribute);
            }
            return result;
        }
    }
}
