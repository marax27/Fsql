using System.Diagnostics;
using Fsql.Cli.Models;
using Fsql.Cli.Settings;
using Fsql.Cli.Terminal;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem;
using Fsql.Core.QueryLanguage;

Console.WriteLine("Greetings.");

var access = new FileSystemAccess();
var parser = new QueryParser();

var readInputStrategy = new ReadOneLineInputStrategy();
var columnWidthStrategy = new DynamicColumnWidthStrategy();

while (true)
{
    var input = readInputStrategy.Read();
    if (input is null)
        break;

#if (DEBUG)
    var result = HandleQuery(input);
    DisplayQueryResults(result);
#elif (RELEASE)
    try
    {
        var result = HandleQuery(input);
        DisplayQueryResults(result);
    }
    catch (ParserException exc)
    {
        ConsoleUtilities.PrintColor($"Parser exception: {exc.Message}", ConsoleColor.Yellow);
        foreach (var error in exc.Errors ?? new List<string>())
            ConsoleUtilities.PrintColor("    - " + error, ConsoleColor.Yellow);
    }
    catch (ApplicationException exc)
    {
        ConsoleUtilities.PrintColor($"Application exception: {exc.Message}", ConsoleColor.Yellow);
        ConsoleUtilities.PrintColor(exc.StackTrace ?? "", ConsoleColor.Yellow);
    }
    catch (Exception exc)
    {
        ConsoleUtilities.PrintColor($"Unhandled exception: {exc.Message}", ConsoleColor.Red);
        Console.WriteLine(exc.StackTrace);
    }
#endif
}

QueryEvaluationResult HandleQuery(string input)
{
    var sw = new Stopwatch();
    sw.Start();

    var query = parser.Parse(input);
    var qe = new QueryEvaluation(access);
    var result = qe.Evaluate(query);

    sw.Stop();

    Console.WriteLine($"{result.Rows.Count} row(s) in {sw.ElapsedMilliseconds}ms.");
    return result;
}

void DisplayQueryResults(QueryEvaluationResult evaluationResult)
{

    var rows = evaluationResult.Rows
        .Select(row => row.Select(value => value.ToText()).ToArray())
        .ToArray();
    var tableModel = new TableModel(evaluationResult.AttributeNames.ToArray(), rows);

    var tableView = new TableView(columnWidthStrategy);
    foreach (var line in tableView.Render(tableModel))
        Console.WriteLine(line);
}
