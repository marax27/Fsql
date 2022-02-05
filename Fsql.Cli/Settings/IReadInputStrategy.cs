namespace Fsql.Cli.Settings;

public interface IReadInputStrategy
{
    string? Read();
}

public abstract class BaseReadInputStrategy : IReadInputStrategy
{
    public abstract string? Read();

    protected string? ReadLine()
    {
        Console.Write("?> ");
        return Console.ReadLine();
    }

    protected bool RequestedQuit(string input) =>
        input.Equals("quit", StringComparison.OrdinalIgnoreCase);
}

public class ReadOneLineInputStrategy : BaseReadInputStrategy
{
    public override string? Read()
    {
        var input = ReadLine();
        if (input is null || RequestedQuit(input))
            return null;
        return input;
    }
}

public class ReadMultiLineInputStrategy : BaseReadInputStrategy
{
    public override string? Read()
    {
        var inputLines = new List<string>();
        while (true)
        {
            var input = ReadLine();

            if (input is null)
                return null;
            else if (inputLines.Count == 0 && RequestedQuit(input))
                return null;
            else if (input.Equals(""))
                break;
            else
                inputLines.Add(input);
        }
        return string.Join('\n', inputLines);
    }
}
