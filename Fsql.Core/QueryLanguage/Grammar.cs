﻿using CSharpParserGenerator;

namespace Fsql.Core.QueryLanguage
{
    internal enum Alphabet
    {
        // Terminal symbols.
        IgnoreToken,
        Select,
        From,
        Where,
        Order,
        By,
        Ascending,
        Descending,
        Separator,
        SingleQuoteString,
        DoubleQuoteString,
        PathString,
        Identifier,
        Wildcard,

        EqualsOperator,

        // Non-terminal symbols.
        QUERY, TERM, TERMS, STRING, EXPRESSION,
        SELECT_EXPRESSION, FROM_EXPRESSION, WHERE_EXPRESSION, ORDER_BY_EXPRESSION,
        ORDER_CONDITION,
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
                [Alphabet.Order] = "[oO][rR][dD][eE][rR]",
                [Alphabet.By] = "[bB][yY]",
                [Alphabet.Ascending] = "[aA][sS][cC]",
                [Alphabet.Descending] = "[dD][eE][sS][cC]",
                [Alphabet.Separator] = ",",
                [Alphabet.SingleQuoteString] = "'[^']*'",
                [Alphabet.DoubleQuoteString] = "\"[^\"]*\"",
                [Alphabet.PathString] = @"(\.|[a-zA-Z]:|/|\\)\S*",
                [Alphabet.Identifier] = "[a-zA-Z_]\\w*",
                [Alphabet.Wildcard] = "\\*",
                [Alphabet.EqualsOperator] = "=",
            });

        public GrammarRules<Alphabet> Rules = new(
            new Dictionary<Alphabet, Token[][]>
            {
                [Alphabet.QUERY] = new []
                {
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], OrderByExpression.NoOrdering);
                    }) },
                    new Token[]{ Alphabet.SELECT_EXPRESSION, Alphabet.FROM_EXPRESSION, Alphabet.ORDER_BY_EXPRESSION, new Op(o =>
                    {
                        o[0] = new Query(o[0], o[1], o[2]);
                    }) },
                },
                [Alphabet.SELECT_EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.Select, Alphabet.TERMS, new Op(o => { o[0] = o[1]; }) },
                },
                [Alphabet.FROM_EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.From, Alphabet.STRING, new Op(o => { o[0] = o[1]; }) },
                },
                [Alphabet.WHERE_EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.Where, Alphabet.EXPRESSION },
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
                        o[0] = new OrderCondition(new Identifier(o[0]), true);
                    }) },
                    new Token[]{ Alphabet.Identifier, Alphabet.Ascending, new Op(o =>
                    {
                        o[0] = new OrderCondition(new Identifier(o[0]), true);
                    }) },
                    new Token[]{ Alphabet.Identifier, Alphabet.Descending, new Op(o =>
                    {
                        o[0] = new OrderCondition(new Identifier(o[0]), false);
                    }) },
                },
                [Alphabet.EXPRESSION] = new []
                {
                    new Token[]{ Alphabet.Identifier, new Op(o =>
                    {
                        o[0] = new IdentifierReferenceExpression(new Identifier(o[0]));
                    }) },
                    new Token[]{ Alphabet.EXPRESSION, Alphabet.EqualsOperator, Alphabet.EXPRESSION, new Op(o =>
                    {
                        o[0] = new EqualsExpression(o[0], o[2]);
                    }) },
                },
                [Alphabet.STRING] = new []
                {
                    new Token[]{ Alphabet.Identifier },
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
                [Alphabet.TERMS] = new []
                {
                    new Token[]{ Alphabet.TERM, new Op(o =>
                    {
                        o[0] = new List<Identifier>{ new Identifier(o[0]) };
                    }) },
                    new Token[]{ Alphabet.TERM, Alphabet.Separator, Alphabet.TERMS, new Op(o =>
                    {
                        var terms = new List<Identifier>{ new Identifier(o[0]) };
                        terms.AddRange(o[2]);
                        o[0] = terms;
                    }) }
                },
                [Alphabet.TERM] = new []
                {
                    new Token[]{ Alphabet.Identifier },
                    new Token[]{ Alphabet.Wildcard },
                }
            });
    }
}
