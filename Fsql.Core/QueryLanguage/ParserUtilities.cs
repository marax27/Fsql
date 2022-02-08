namespace Fsql.Core.QueryLanguage;

public class ParserUtilities
{
    private static readonly IReadOnlyDictionary<char, double> Multipliers = new Dictionary<char, double>
    {
        { 'k', 1024.0 },
        { 'm', 1024.0 * 1024.0 },
        { 'g', 1024.0 * 1024.0 * 1024.0 },
        { 't', 1024.0 * 1024.0 * 1024.0 * 1024.0 },
    };

    public static double ParseNumber(string value)
    {
        var potentialMultiplier = value[^1];
        if (char.IsLetter(potentialMultiplier))
        {
            var key = char.ToLower(potentialMultiplier);
            if (!Multipliers.ContainsKey(key))
                throw new ApplicationException($"Unsupported number multiplier: '{potentialMultiplier}'. Supported multipliers: " + string.Join(", ", Multipliers.Keys));

            return double.Parse(value[..^1]) * Multipliers[key];
        }

        return double.Parse(value);
    }
}
