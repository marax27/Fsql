using Fsql.Core;
using Fsql.Core.FileSystem;

Console.WriteLine("Greetings.");

var access = new FileSystemAccess();

while (true)
{
    Console.Write("?> ");
    var input = Console.ReadLine();
    if (input is null || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    var query = new Query(input);
    var qe = new QueryEvaluation(access);

    var result = qe.Evaluate(query);
    DisplayEvaluationResults(result);
}

void DisplayEvaluationResults(QueryEvaluationResult evaluationResult)
{
    var attributeNames = evaluationResult.AttributeNames;
    Console.WriteLine(FormatRow(attributeNames));

    Console.WriteLine(new string('-', 100));

    foreach (var row in evaluationResult.Rows)
    {
        Console.WriteLine(FormatRow(row.Select(value => value.ToText())));
    }

    string FormatRow(IEnumerable<string> values)
        => string.Join(" | ", values.Select(value => value.PadRight(25)));
}
