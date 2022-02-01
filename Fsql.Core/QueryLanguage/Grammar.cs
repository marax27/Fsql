using CSharpParserGenerator;

namespace Fsql.Core.QueryLanguage
{
    internal enum Alphabet
    {
        // Terminal symbols.
        IgnoreToken,
        Select,
        From,
        Separator,
        SingleQuoteString,
        DoubleQuoteString,
        SimpleString,
        Wildcard,

        // Non-terminal symbols.
        QUERY, STRING
    }

    internal class Grammar
    {
        public LexerDefinition<Alphabet> Tokens = new(
            new Dictionary<Alphabet, TokenRegex>
            {
                [Alphabet.IgnoreToken] = "[ \\n]+",
                [Alphabet.Select] = "[sS][eE][lL][eE][cC][tT]",
                [Alphabet.From] = "[fF][rR][oO][mM]",
                [Alphabet.Separator] = ",",
                [Alphabet.SingleQuoteString] = "'[^']*'",
                [Alphabet.DoubleQuoteString] = "\"[^\"]*\"",
                [Alphabet.SimpleString] = @"[a-zA-Z_\\/\.]\S*",
                [Alphabet.Wildcard] = "\\*"
            });

        public GrammarRules<Alphabet> Rules = new(
            new Dictionary<Alphabet, Token[][]>
            {
                [Alphabet.QUERY] = new []
                {
                    new Token[]{ Alphabet.Select, Alphabet.Wildcard, Alphabet.From, Alphabet.STRING, new Op(o =>
                    {
                        o[0] = new Query(new List<string>{ o[1] }, o[3]);
                    }) }
                },
                [Alphabet.STRING] = new []
                {
                    new Token[]{ Alphabet.SimpleString },
                    new Token[]{ Alphabet.SingleQuoteString, new Op(o =>
                    {
                        var value = (o[0] as string)?[1..^1];
                        o[0] = value ?? throw new ParserException($"Failed to process {nameof(Alphabet.SingleQuoteString)}: null string.");
                    }) },
                    new Token[]{ Alphabet.DoubleQuoteString, new Op(o =>
                    {
                        var value = (o[0] as string)?[1..^1];
                        o[0] = value ?? throw new ParserException($"Failed to process {nameof(Alphabet.DoubleQuoteString)}: null string.");
                    }) }
                },
            });
    }
}
