using Fsql.Core.FileSystem;

namespace Fsql.Core.Evaluation;

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

    private static BaseValueType[] CreateRow(BaseFileSystemEntry entry, IReadOnlyCollection<string> attributes, IQueryContext<BaseFileSystemEntry> queryContext)
    {
        var result = attributes
            .Select(attribute => queryContext.Get(attribute, entry))
            .ToArray();
        return result;
    }

    private static IReadOnlyCollection<string> ExpandAttributes(IEnumerable<string> attributes, IQueryContext<BaseFileSystemEntry> queryContext)
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