using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem.Abstractions;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluating
{
    public class WhenEvaluatingAttributeNames
    {
        [Theory]
        [InlineData("name")]
        [InlineData("Name")]
        [InlineData("NAME")]
        public void Given1NamedAttributeReturnExpectedHeader(string givenAttribute)
        {
            var result = Evaluate(new[] { givenAttribute });
            result.AttributeNames.Should().BeEquivalentTo(new[] { givenAttribute });
        }

        [Fact]
        public void Given3NamedAttributesReturnExpectedHeaders()
        {
            var result = Evaluate(new []{ "Name", "Extension", "Type" });
            result.AttributeNames.Should().BeEquivalentTo(new[] { "Name", "Extension", "Type" },
                o => o.WithStrictOrdering());
        }

        [Fact]
        public void Given3NamedAttributesInDifferentOrderReturnExpectedHeaders()
        {
            var result = Evaluate(new[] { "Extension", "Type", "Name" });
            result.AttributeNames.Should().BeEquivalentTo(new[] { "Extension", "Type", "Name" },
                o => o.WithStrictOrdering());
        }

        [Theory]
        [InlineData("name")]
        [InlineData("Name")]
        [InlineData("NAME")]
        [InlineData("extension")]
        [InlineData("type")]
        public void Given1NamedAttributeReturn1ValuePerRow(string givenAttribute)
        {
            var result = Evaluate(new[] { givenAttribute });
            result.Rows.Should().OnlyContain(row => row.Length == 1);
        }

        [Theory]
        [InlineData("name")]
        [InlineData("Name")]
        [InlineData("NAME")]
        public void GivenFilenameAttributeReturnExpectedValues(string givenAttribute)
        {
            var result = Evaluate(new[] { givenAttribute });
            var actualValues = result.Rows.Select(row => row[0].ToText());
            actualValues.Should().BeEquivalentTo(new[] { "1.txt", "2.txt", "3.jpg", "sub.dir" },
                o => o.WithStrictOrdering());
        }

        [Fact]
        public void GivenExtensionAttributeReturnExpectedValues()
        {
            var result = Evaluate(new[] { "extension" });
            var actualValues = result.Rows.Select(row => row[0].ToText());
            actualValues.Should().BeEquivalentTo(new[] { ".txt", ".txt", ".jpg", "null" },
                o => o.WithStrictOrdering());
        }

        private static QueryEvaluationResult Evaluate(IReadOnlyCollection<string> attributeNames)
        {
            var attributes = attributeNames.Select(a => new Identifier(a)).ToList();
            var fsAccess = CreateFileSystemAccess();
            var query = new Query(attributes, new("sample-path", false), null, OrderByExpression.NoOrdering);
            var sut = new QueryEvaluation(fsAccess);

            return sut.Evaluate(query);
        }

        private static IFileSystemAccess CreateFileSystemAccess()
            => new FakeFileSystemAccess()
                .WithFiles("sample-path", 100, "1.txt", "2.txt", "3.jpg")
                .WithDirectories("sample-path", 0, "sub.dir/");
    }
}
