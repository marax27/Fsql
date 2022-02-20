using CSharpParserGenerator;
using Fsql.Core.DataStructures;

namespace Fsql.Core.QueryLanguage
{
    internal enum Alphabet
    {
        // Terminal symbols.
        IgnoreToken,
        Select,
        From,
        Where,
        Group,
        Order,
        By,
        And,
        Or,
        Ascending,
        Descending,
        Not,
        Like,
        Recursive,
        Separator,
        Number,
        SingleQuoteString,
        DoubleQuoteString,
        PathString,
        Identifier,
        Wildcard,

        EqualsOperator,
        NotEqualOperator,
        GreaterThanOperator,
        LessThanOperator,
        GreaterThanOrEqualOperator,
        LessThanOrEqualOperator,
        LeftParenthesis,
        RightParenthesis,

        // Non-terminal symbols.
        QUERY, SELECT_TERMS, STRING,
        EXPRESSION, A1, A4, A6, A7,
        SELECT_EXPRESSION, FROM_EXPRESSION, WHERE_EXPRESSION, GROUP_BY_EXPRESSION, ORDER_BY_EXPRESSION,
        ORDER_CONDITION, FROM_PATH, FUNCTION_CALL, FUNCTION_ARGUMENTS,
    }

    internal class Grammar
    {
        public LexerDefinition<Alphabet> Tokens = new(
            new Dictionary<Alphabet, TokenRegex>
            {
                [Alphabet.IgnoreToken] = "[ \\n\\t]+",
                [Alphabet.Select] = "[sS][eE][lL][eE][cC][tT]",
                [Alphabet.From] = "[fF][rR][oO][mM]",
                [Alphabet.Where] = "[wW][hH][eE][rR][eE]",
                [Alphabet.Group] = "[gG][rR][oO][uU][pP]",
                [Alphabet.Order] = "[oO][rR][dD][eE][rR]",
                [Alphabet.By] = "[bB][yY]",
                [Alphabet.And] = "[aA][nN][dD]",
                [Alphabet.Or] = "[oO][rR]",
                [Alphabet.Ascending] = "[aA][sS][cC]",
                [Alphabet.Descending] = "[dD][eE][sS][cC]",
                [Alphabet.Not] = "[nN][oO][tT]",
                [Alphabet.Like] = "[lL][iI][kK][eE]",
                [Alphabet.Recursive] = "[rR][eE][cC][uU][rR][sS][iI][vV][eE]",
                [Alphabet.Separator] = ",",
                [Alphabet.Number] = "[+-]?([0-9]*[.])?[0-9]+[a-zA-Z]?",
                [Alphabet.SingleQuoteString] = "'[^']*'",
                [Alphabet.DoubleQuoteString] = "\"[^\"]*\"",
                [Alphabet.PathString] = @"(\.|[a-zA-Z]:|/|\\)\S*",
                [Alphabet.Identifier] = "[a-zA-Z_]\\w*",
                [Alphabet.Wildcard] = "\\*",
                [Alphabet.EqualsOperator] = "=",
                [Alphabet.NotEqualOperator] = "(<>|!=)",
                [Alphabet.GreaterThanOperator] = ">",
                [Alphabet.LessThanOperator] = "<",
                [Alphabet.GreaterThanOrEqualOperator] = ">=",
                [Alphabet.LessThanOrEqualOperator] = "<=",
                [Alphabet.LeftParenthesis] = "\\(",
                [Alphabet.RightParenthesis] = "\\)",
            });

        public GrammarRules<Alphabet> Rules = new(
            new Dictionary<Alphabet, Token[][]>
            {
                [Alphabet.QUERY] = new []
                {
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], null, GroupByExpression.NoGrouping, OrderByExpression.NoOrdering);
                    }) },
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, Alphabet.WHERE_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], o[2], GroupByExpression.NoGrouping, OrderByExpression.NoOrdering);
                    }) },
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, Alphabet.ORDER_BY_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], null, GroupByExpression.NoGrouping, o[2]);
                    }) },
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, Alphabet.WHERE_EXPRESSION, Alphabet.ORDER_BY_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], o[2], GroupByExpression.NoGrouping, o[3]);
                    }) },
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, Alphabet.GROUP_BY_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], null, o[2], OrderByExpression.NoOrdering);
                    }) },
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, Alphabet.WHERE_EXPRESSION, Alphabet.GROUP_BY_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], o[2], o[3], OrderByExpression.NoOrdering);
                    }) },
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, Alphabet.GROUP_BY_EXPRESSION, Alphabet.ORDER_BY_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], null, o[2], o[3]);
                    }) },
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, Alphabet.WHERE_EXPRESSION, Alphabet.GROUP_BY_EXPRESSION, Alphabet.ORDER_BY_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], o[2], o[3], o[4]);
                    }) },
                },
                [Alphabet.SELECT_EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.Select, Alphabet.SELECT_TERMS, new Op(o => { o[0] = o[1]; }) },
                },
                [Alphabet.FROM_EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.From, Alphabet.FROM_PATH, new Op(o => { o[0] = new FromExpression(o[1], false); }) },
                    new Token[]{ Alphabet.From, Alphabet.FROM_PATH, Alphabet.Recursive, new Op(o => { o[0] = new FromExpression(o[1], true); }) },
                },
                [Alphabet.FROM_PATH] = new []
                {
                    new Token[]{ Alphabet.STRING },
                    new Token[]{ Alphabet.Identifier },
                },
                [Alphabet.WHERE_EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.Where, Alphabet.EXPRESSION, new Op(o =>
                    {
                        o[0] = o[1];
                    }) },
                },
                [Alphabet.GROUP_BY_EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.Group, Alphabet.By, Alphabet.Identifier, new Op(o =>
                    {
                        var expression = new IdentifierReferenceExpression(new(o[2]));
                        o[0] = new GroupByExpression(new List<Expression>{ expression });
                    }) },
                    new Token[]{ Alphabet.Group, Alphabet.By, Alphabet.Identifier, Alphabet.LeftParenthesis, Alphabet.Identifier, Alphabet.RightParenthesis, new Op(o =>
                    {
                        var arguments = new[] { new IdentifierReferenceExpression(new(o[4])) };
                        var expression = new FunctionCall(new(o[2]), new(arguments));
                        o[0] = new GroupByExpression(new []{ expression });
                    }) },
                },
                [Alphabet.ORDER_BY_EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.Order, Alphabet.By, Alphabet.ORDER_CONDITION, new Op(o =>
                    {
                        o[0] = new OrderByExpression(new List<OrderCondition>{ o[2] });
                    }) },
                },
                [Alphabet.ORDER_CONDITION] = new []
                {
                    new Token[]{ Alphabet.Identifier, new Op(o =>
                    {
                        o[0] = new OrderCondition(new IdentifierReferenceExpression(new(o[0])), true);
                    }) },
                    new Token[]{ Alphabet.Identifier, Alphabet.Ascending, new Op(o =>
                    {
                        o[0] = new OrderCondition(new IdentifierReferenceExpression(new(o[0])), true);
                    }) },
                    new Token[]{ Alphabet.Identifier, Alphabet.Descending, new Op(o =>
                    {
                        o[0] = new OrderCondition(new IdentifierReferenceExpression(new(o[0])), false);
                    }) },
                    new Token[]{ Alphabet.FUNCTION_CALL, new Op(o =>
                    {
                        o[0] = new OrderCondition(o[0], true);
                    }) },
                    new Token[]{ Alphabet.FUNCTION_CALL, Alphabet.Ascending, new Op(o =>
                    {
                        o[0] = new OrderCondition(o[0], true);
                    }) },
                    new Token[]{ Alphabet.FUNCTION_CALL, Alphabet.Descending, new Op(o =>
                    {
                        o[0] = new OrderCondition(o[0], false);
                    }) },
                },
                [Alphabet.EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.A7 }
                },
                [Alphabet.A7] = new[]
                {
                    new Token[] { Alphabet.A7, Alphabet.Or, Alphabet.A6, new Op(o =>
                    {
                        o[0] = new OrExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A6 },
                },
                [Alphabet.A6] = new []
                {
                    new Token[] { Alphabet.A6, Alphabet.And, Alphabet.A4, new Op(o =>
                    {
                        o[0] = new AndExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A4 },
                },
                [Alphabet.A4] = new []
                {
                    new Token[] { Alphabet.A4, Alphabet.EqualsOperator, Alphabet.A1, new Op(o =>
                    {
                        o[0] = new EqualsExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A4, Alphabet.NotEqualOperator, Alphabet.A1, new Op(o =>
                    {
                        o[0] = new NotEqualExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A4, Alphabet.GreaterThanOperator, Alphabet.A1, new Op(o =>
                    {
                        o[0] = new GreaterThanExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A4, Alphabet.LessThanOperator, Alphabet.A1, new Op(o =>
                    {
                        o[0] = new LessThanExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A4, Alphabet.GreaterThanOrEqualOperator, Alphabet.A1, new Op(o =>
                    {
                        o[0] = new GreaterThanOrEqualExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A4, Alphabet.LessThanOrEqualOperator, Alphabet.A1, new Op(o =>
                    {
                        o[0] = new LessThanOrEqualExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A4, Alphabet.Like, Alphabet.A1, new Op(o =>
                    {
                        o[0] = new LikeOperatorExpression(o[0], o[2]);
                    }) },
                    new Token[] { Alphabet.A4, Alphabet.Not, Alphabet.Like, Alphabet.A1, new Op(o =>
                    {
                        o[0] = new NotLikeOperatorExpression(o[0], o[3]);
                    }) },
                    new Token[] { Alphabet.A1 },
                },
                [Alphabet.A1] = new []
                {
                    new Token[]{ Alphabet.FUNCTION_CALL },
                    new Token[] { Alphabet.LeftParenthesis, Alphabet.A7, Alphabet.RightParenthesis, new Op(o => { o[0] = o[1]; }) },
                    new Token[] { Alphabet.Identifier, new Op(o => { o[0] = new IdentifierReferenceExpression(new(o[0])); }) },
                    new Token[] { Alphabet.Number, new Op(o => { o[0] = new NumberConstant(ParserUtilities.ParseNumber(o[0])); }) },
                    new Token[] { Alphabet.STRING, new Op(o => { o[0] = new StringConstant(o[0]); }) },
                },
                [Alphabet.STRING] = new []
                {
                    new Token[]{ Alphabet.PathString },
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
                [Alphabet.FUNCTION_CALL] = new []
                {
                    new Token[]{ Alphabet.Identifier, Alphabet.LeftParenthesis, Alphabet.Identifier, Alphabet.RightParenthesis, new Op(o =>
                    {
                        var items = new[] { new IdentifierReferenceExpression(new(o[2])) };
                        o[0] = new FunctionCall(new Identifier(o[0]), new(items));
                    }) },
                },
                [Alphabet.SELECT_TERMS] = new []
                {
                    new Token[]{ Alphabet.Identifier, new Op(o =>
                    {
                        var term = new IdentifierReferenceExpression(new(o[0]));
                        o[0] = new List<Expression>{ term };
                    }) },
                    new Token[]{ Alphabet.Wildcard, new Op(o =>
                    {
                        var term = new IdentifierReferenceExpression(new(o[0]));
                        o[0] = new List<Expression>{ term };
                    }) },
                    new Token[]{ Alphabet.Identifier, Alphabet.LeftParenthesis, Alphabet.Identifier, Alphabet.RightParenthesis, new Op(o =>
                    {
                        var items = new[] { new IdentifierReferenceExpression(new(o[2])) };
                        var term = new FunctionCall(new Identifier(o[0]), new(items));
                        o[0] = new List<Expression>{ term };
                    }) },
                    new Token[]{ Alphabet.Identifier, Alphabet.Separator, Alphabet.SELECT_TERMS, new Op(o =>
                    {
                        var term = new IdentifierReferenceExpression(new(o[0]));
                        o[0] = new List<Expression>{ term };
                        o[0].AddRange(o[2]);
                    }) },
                    new Token[]{ Alphabet.Wildcard, Alphabet.Separator, Alphabet.SELECT_TERMS, new Op(o =>
                    {
                        var term = new IdentifierReferenceExpression(new(o[0]));
                        o[0] = new List<Expression>{ term };
                        o[0].AddRange(o[2]);
                    }) },
                    new Token[]{ Alphabet.Identifier, Alphabet.LeftParenthesis, Alphabet.Identifier, Alphabet.RightParenthesis, Alphabet.Separator, Alphabet.SELECT_TERMS, new Op(o =>
                    {
                        var items = new[] { new IdentifierReferenceExpression(new(o[2])) };
                        var term = new FunctionCall(new Identifier(o[0]), new(items));
                        o[0] = new List<Expression>{ term };
                        o[0].AddRange(o[5]);
                    }) },
                },
            });
    }
}
