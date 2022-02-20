using System;
using Fsql.Core.QueryLanguage;
using Xunit;

namespace Fsql.Core.Tests.WhenParsing;

/// <summary>
/// Query Parser should be shared between tests if possible.
/// Parser compilation is time-consuming according to library docs.
/// </summary>
public sealed class ParserFixture : IDisposable
{
    public QueryParser Sut { get; }

    public ParserFixture()
    {
        Sut = new QueryParser();
    }

    public void Dispose()
    {

    }
}

[CollectionDefinition("Parser test collection")]
public sealed class ParserTestCollection : ICollectionFixture<ParserFixture>
{

}
