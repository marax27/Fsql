using System;
using System.Collections.Generic;
using Fsql.Core.Evaluation;

namespace Fsql.Core.Tests.WhenEvaluatingExpressions.RelationalOperators;

public record DataTypeComparisonTestCase(BaseValueType Left, BaseValueType Right);

public class DataTypeComparisonDatasets
{
    public static IEnumerable<DataTypeComparisonTestCase> EqualValuesOfTheSameType => new DataTypeComparisonTestCase[]
    {
        new(new NumberValueType(123.0), new NumberValueType(123.0)),
        new(new NumberValueType(-1024), new NumberValueType(-1024)),
        new(new NumberValueType(0.314771), new NumberValueType(0.314771)),
        new(new NumberValueType(1e9), new NumberValueType(1e9)),
        new(new NumberValueType(-6e-6), new NumberValueType(-6e-6)),
        new(new NumberValueType(0.0), new NumberValueType(-0.0)),
        new(new NumberValueType(double.MinValue), new NumberValueType(double.MinValue)),
        new(new NumberValueType(double.MaxValue), new NumberValueType(double.MaxValue)),

        new(new StringValueType("sample string"), new StringValueType("sample string")),
        new(new StringValueType(""), new StringValueType("")),
        new(new StringValueType("UPPER and lower Case."), new StringValueType("UPPER and lower Case.")),
        new(new StringValueType("\\things\\ %20s"), new StringValueType("\\things\\ %20s")),
        new(new StringValueType("zażółć gęślą jaźń"), new StringValueType("zażółć gęślą jaźń")),

        new(new DateTimeValueType(new(2012, 5, 30, 21, 45, 59)), new DateTimeValueType(new(2012, 5, 30, 21, 45, 59))),
        new(new DateTimeValueType(new(1999, 12, 31)), new DateTimeValueType(new(1999, 12, 31))),
        new(new DateTimeValueType(DateTime.MinValue), new DateTimeValueType(DateTime.MinValue)),
        new(new DateTimeValueType(DateTime.MaxValue), new DateTimeValueType(DateTime.MaxValue)),

        new(new BooleanValueType(true), new BooleanValueType(true)),
        new(new BooleanValueType(false), new BooleanValueType(false)),
    };

    public static IEnumerable<DataTypeComparisonTestCase> FirstValueSmallerOfTheSameType => new DataTypeComparisonTestCase[]
    {
        new(new NumberValueType(-1024), new NumberValueType(1024)),
        new(new NumberValueType(1024), new NumberValueType(1024.01)),
        new(new NumberValueType(1023.99), new NumberValueType(1024)),
        new(new NumberValueType(-150000), new NumberValueType(1)),
        new(new NumberValueType(2000300040005000), new NumberValueType(2000300040005001)),
        new(new NumberValueType(391.123455), new NumberValueType(391.123456)),
        new(new NumberValueType(0.0), new NumberValueType(1e-15)),
        new(new NumberValueType(-1e-15), new NumberValueType(0.0)),
        new(new NumberValueType(double.MinValue), new NumberValueType(double.MaxValue)),

        new(new StringValueType("example string."), new StringValueType("very different string.")),
        new(new StringValueType("10001"), new StringValueType("10002")),
        new(new StringValueType("11002"), new StringValueType("12001")),
        new(new StringValueType("ABCDEF"), new StringValueType("abcdef")),
        new(new StringValueType(""), new StringValueType(" ")),
        new(new StringValueType("     "), new StringValueType("       ")),
        new(new StringValueType("example01.pdf"), new StringValueType("example02.pdf")),
        new(new StringValueType("example01.pdf"), new StringValueType("example01.wmv")),
        new(new StringValueType("example01.pdf"), new StringValueType("example01.pdf.1")),
        new(new StringValueType("example01.PDF"), new StringValueType("example01.pdf")),
        new(new StringValueType("example01"), new StringValueType("example01.pdf")),

        new(new DateTimeValueType(new(2012, 5, 30, 10, 45, 58)), new DateTimeValueType(new(2012, 5, 30, 10, 45, 59))),
        new(new DateTimeValueType(new(1998, 12, 5)), new DateTimeValueType(new(1999, 12, 5))),
        new(new DateTimeValueType(new(1999, 11, 5)), new DateTimeValueType(new(1999, 12, 5))),
        new(new DateTimeValueType(new(1999, 12, 5)), new DateTimeValueType(new(1999, 12, 6))),
        new(new DateTimeValueType(DateTime.MinValue), new DateTimeValueType(DateTime.MaxValue)),
        
        new(new BooleanValueType(false), new BooleanValueType(true)),
    };

    public static IEnumerable<DataTypeComparisonTestCase> ValuesAgainstNull => new DataTypeComparisonTestCase[]
    {
        new(new NumberValueType(9.5), new NullValueType()),
        new(new NumberValueType(0), new NullValueType()),
        new(new NumberValueType(-1e20), new NullValueType()),
        new(new NumberValueType(double.MinValue), new NullValueType()),
        new(new NumberValueType(double.MaxValue), new NullValueType()),

        new(new StringValueType("sample text"), new NullValueType()),
        new(new StringValueType(""), new NullValueType()),

        new(new BooleanValueType(true), new NullValueType()),
        new(new BooleanValueType(false), new NullValueType()),

        new(new DateTimeValueType(new(2001, 10, 30, 22, 59, 59)), new NullValueType()),
        new(new DateTimeValueType(DateTime.MinValue), new NullValueType()),
        new(new DateTimeValueType(DateTime.MaxValue), new NullValueType()),

        new(new NullValueType(), new NullValueType()),
    };
}
