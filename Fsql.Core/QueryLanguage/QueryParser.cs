using CSharpParserGenerator;

namespace Fsql.Core.QueryLanguage
{
    public class QueryParser
    {
        private readonly Parser<Alphabet> _parser;

        public QueryParser()
        {
            var grammar = new Grammar();
            var lexer = new Lexer<Alphabet>(grammar.Tokens, Alphabet.IgnoreToken);
            _parser = new ParserGenerator<Alphabet>(lexer, grammar.Rules).CompileParser();
        }

        public Query Parse(string queryCode)
        {
            var result = _parser.Parse<Query>(queryCode);
            AssertParse(result);
            return result.Value;
        }

        private void AssertParse(ParseResult<Query> result)
        {
            if (result.Success && result.Value is not null)
                return;

            var errors = result.Errors;
            var message = errors.Count == 1
                ? FormatErrorInfo(errors[0])
                : $"{errors.Count} parsing errors.";

            throw new ParserException(message, errors.Select(FormatErrorInfo).ToList());
        }

        private string FormatErrorInfo(ErrorInfo entry)
            => $"[{entry.Type}] {entry.Description}";
    }
}
