using System.Globalization;

namespace Fsql.Core.Evaluation
{
    public enum DataTypes
    {
        Null,
        String,
        Number,
        DateTime,
        Boolean,
    }

    public abstract record BaseValueType : IComparable<BaseValueType>
    {
        public abstract string ToText();

        public abstract int CompareTo(BaseValueType? other);

        public abstract bool EvaluatesToTrue();

        public abstract DataTypes Type { get; }

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
                StringValueType stringOther => string.Compare(Value, stringOther.Value, StringComparison.Ordinal),
                _ => TryNullOrThrow(other, nameof(StringValueType))
            };

        public override bool EvaluatesToTrue() => Value.Length != 0;

        public override DataTypes Type => DataTypes.String;
    }

    public sealed record NullValueType : BaseValueType
    {
        public override string ToText() => "null";

        public override int CompareTo(BaseValueType? other) => 1;
     
        public override bool EvaluatesToTrue() => false;

        public override DataTypes Type => DataTypes.Null;
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

        public override bool EvaluatesToTrue() => Value != 0.0;

        public override DataTypes Type => DataTypes.Number;
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

        public override bool EvaluatesToTrue() => true;

        public override DataTypes Type => DataTypes.DateTime;
    }

    public sealed record BooleanValueType(bool Value) : BaseValueType
    {
        public override string ToText() => Value ? "T" : "F";

        public override int CompareTo(BaseValueType? other) =>
            other switch
            {
                BooleanValueType booleanValue => Value == booleanValue.Value
                    ? 0
                    : (Value ? 1 : -1),
                _ => TryNullOrThrow(other, nameof(BooleanValueType))
            };

        public override bool EvaluatesToTrue() => Value;

        public override DataTypes Type => DataTypes.Boolean;
    }

    public class CastException : ApplicationException
    {
        public CastException(string message) : base(message) {}
    }
}
