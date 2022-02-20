using System.Collections;

namespace Fsql.Core.DataStructures;

public class InRecordList<T> : IReadOnlyList<T>, IEquatable<InRecordList<T>>
{
    private readonly IReadOnlyList<T> _internal;

    public InRecordList(IEnumerable<T> items)
    {
        _internal = items.ToArray();
    }

    public IEnumerator<T> GetEnumerator() =>
        _internal.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)_internal).GetEnumerator();

    public int Count => _internal.Count;

    public T this[int index] => _internal[index];

    public bool Equals(InRecordList<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _internal.SequenceEqual(other._internal);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((InRecordList<T>)obj);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        foreach (var item in _internal)
            hash.Add(item);
        return hash.ToHashCode();
    }

    public static bool operator ==(InRecordList<T>? left, InRecordList<T>? right) =>
        Equals(left, right);

    public static bool operator !=(InRecordList<T>? left, InRecordList<T>? right) =>
        !Equals(left, right);
}
