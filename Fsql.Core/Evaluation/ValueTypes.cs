using System.Globalization;

namespace Fsql.Core.Evaluation
{
    public abstract record BaseValueType
    {
        public abstract string ToText();
    }

    public sealed record StringValueType(string Value) : BaseValueType
    {
        public override string ToText() => Value;
    }

    public sealed record NullValueType : BaseValueType
    {
        public override string ToText() => "null";
    }

    public sealed record NumberValueType(double Value) : BaseValueType
    {
        public override string ToText() => Value.ToString(CultureInfo.CurrentCulture);
    }

    public sealed record DateTimeValueType(DateTime DateTime) : BaseValueType
    {
        public override string ToText() => DateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
