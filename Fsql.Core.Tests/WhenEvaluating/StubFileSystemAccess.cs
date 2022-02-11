using System.Collections.Generic;
using Fsql.Core.FileSystem.Abstractions;

namespace Fsql.Core.Tests.WhenEvaluating;

internal class StubFileSystemAccess : IFileSystemAccess
{
    private readonly IEnumerable<BaseFileSystemEntry> _entries;

    public StubFileSystemAccess(IEnumerable<BaseFileSystemEntry> entries)
    {
        _entries = entries;
    }

    public IEnumerable<BaseFileSystemEntry> GetEntries(string directoryPath) => _entries;
}