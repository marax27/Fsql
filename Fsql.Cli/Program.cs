using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem;
using Fsql.Core.QueryLanguage;

Console.WriteLine("Greetings.");

var access = new FileSystemAccess();
var parser = new QueryParser();

while (true)
{
    Console.Write("?> ");
    var input = Console.ReadLine();
    if (input is null || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

#if (DEBUG)
    var query = parser.Parse(input);
    var qe = new QueryEvaluation(access);

    var result = qe.Evaluate(query);
    DisplayQueryResults(result);
#elif (RELEASE)
    try
    {
        var query = parser.Parse(input);
        var qe = new QueryEvaluation(access);

        var result = qe.Evaluate(query);
        DisplayQueryResults(result);
    }
    catch (ParserException exc)
    {
        PrintColor($"Parser exception: {exc.Message}", ConsoleColor.Yellow);
        foreach (var error in exc.Errors ?? new List<string>())
            PrintColor("    - " + error, ConsoleColor.Yellow);
    }
    catch (ApplicationException exc)
    {
        PrintColor($"Application exception: {exc.Message}", ConsoleColor.Yellow);
        PrintColor(exc.StackTrace ?? "", ConsoleColor.Yellow);
    }
    catch (Exception exc)
    {
        PrintColor($"Unhandled exception: {exc.Message}", ConsoleColor.Red);
        Console.WriteLine(exc.StackTrace);
    }
#endif
}

void PrintColor(string message, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ResetColor();
}

void DisplayQueryResults(QueryEvaluationResult evaluationResult)
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
