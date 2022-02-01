namespace Fsql.Core.QueryLanguage
{
    public class ParserException : ApplicationException
    {
        public IReadOnlyList<string>? Errors { get; }

        public ParserException(string message, IReadOnlyList<string>? errors = null)
            : base(message)
        {
            Errors = errors;
        }
    }
}
