namespace Fsql.Core
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
}
