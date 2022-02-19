namespace Fsql.Cli.Terminal;

internal class ConsoleUtilities
{
    public static void PrintColor(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
