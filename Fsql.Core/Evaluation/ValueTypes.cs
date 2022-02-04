using System.Globalization;

namespace Fsql.Core.Evaluation
{
    public abstract record BaseValueType : IComparable<BaseValueType>
    {
        public abstract string ToText();

        public abstract int CompareTo(BaseValueType? other);

        protected int TryNullOrThrow(BaseValueType? other, string castTypeName) =>
            other switch
            {
                NullValueType => -1,
                _ => throw new CastException($"Cannot cast {other} as {castTypeName}.")
            };
    }

    public sealed record StringValueType(string Value) : BaseValueType
    {
        public override string ToText() => Value;

        public override int CompareTo(BaseValueType? other) =>
            other switch
            {
                StringValueType stringOther => string.Compare(Value, stringOther.Value, StringComparison.OrdinalIgnoreCase),
                _ => TryNullOrThrow(other, nameof(StringValueType))
            };
    }

    public sealed record NullValueType : BaseValueType
    {
        public override string ToText() => "null";

        public override int CompareTo(BaseValueType? other) => 1;
    }

    public sealed record NumberValueType(double Value) : BaseValueType
    {
        public override string ToText() => Value.ToString(CultureInfo.CurrentCulture);

        public override int CompareTo(BaseValueType? other) =>
            other switch
            {
                NumberValueType numberOther => Value.CompareTo(numberOther.Value),
                _ => TryNullOrThrow(other, nameof(NumberValueType))
            };
    }

    public sealed record DateTimeValueType(DateTime DateTime) : BaseValueType
    {
        public override string ToText() => DateTime.ToString("yyyy-MM-dd HH:mm:ss");

        public override int CompareTo(BaseValueType? other) =>
            other switch
            {
                DateTimeValueType dateTimeValue => DateTime.CompareTo(dateTimeValue.DateTime),
                _ => TryNullOrThrow(other, nameof(DateTime))
            };
    }

    public sealed record BooleanValueType(bool Value) : BaseValueType
    {
        public override string ToText() => Value ? "T" : "F";

        public override int CompareTo(BaseValueType? other) =>
            other switch
            {
                BooleanValueType booleanValue => Value == booleanValue.Value
                    ? booleanValue.Value ? 0 : 1
                    : booleanValue.Value ? -1 : 0,
                _ => TryNullOrThrow(other, nameof(BooleanValueType))
            };
    }

    public class CastException : ApplicationException
    {
        public CastException(string message) : base(message) {}
    }
}
