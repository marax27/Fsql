using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem.Abstractions;
using Xunit;

namespace Fsql.Core.Tests.WhenEvaluating
{
    public class QueryEvaluationTests
    {
        [Fact]
        public void GivenEmptyPathReturnEmpty()
        {
            var givenPath = "./empty_path";
            var fsAccess = new FakeFileSystemAccess()
                .WithFiles(givenPath, 1234);

            var result = Evaluate(givenPath, fsAccess);
            result.Rows.Should().BeEmpty();
        }

        [Fact]
        public void Given1FileAnd1DirectoryReturn2Entries()
        {
            var givenPath = "./path";
            var fsAccess = new FakeFileSystemAccess()
                .WithFiles(givenPath, 500, "file01.txt")
                .WithDirectories(givenPath, 0, "sub_directory");

            var result = Evaluate(givenPath, fsAccess);
            result.Rows.Should().HaveCount(2);
        }

        [Fact]
        public void Given3FilesReturn3Entries()
        {
            var givenPath = "./EXAMPLE_PATH";
            var fsAccess = new FakeFileSystemAccess()
                .WithFiles(givenPath, 200, "00", "11", "22")
                .WithFiles("./other-path", 300, "0a", "1b", "2c", "3d", "4e");

            var result = Evaluate(givenPath, fsAccess);
            result.Rows.Should().HaveCount(3);
        }

        [Fact]
        public void Given3DirectoriesReturn3Entries()
        {
            var givenPath = "./EXAMPLE_PATH";
            var fsAccess = new FakeFileSystemAccess()
                .WithDirectories(givenPath, 1234, "00", "11", "22")
                .WithDirectories("./other-path", 1234, "0a", "1b", "2c", "3d", "4e");

            var result = Evaluate(givenPath, fsAccess);
            result.Rows.Should().HaveCount(3);
        }

        private QueryEvaluationResult Evaluate(string givenPath, IFileSystemAccess fileSystemAccess)
        {
            var givenQuery = new Query(new List<Expression>(), new(givenPath, false), null, GroupByExpression.NoGrouping, OrderByExpression.NoOrdering);
            var sut = new QueryEvaluation(fileSystemAccess);
            return sut.Evaluate(givenQuery);
        }
    }

    internal record FakeFileSystemEntry : BaseFileSystemEntry
    {
        public override double Size { get; }
        public override string AbsolutePath => FullPath;
        public override DateTime AccessTime => DateTime.MinValue;
        public override DateTime CreateTime => DateTime.MinValue;
        public override DateTime ModifyTime => DateTime.MinValue;

        public FakeFileSystemEntry(string fullPath, FileSystemEntryType type, double size)
            : base(fullPath, type)
        {
            Size = size;
        }
    }

    internal class FakeFileSystemAccess : IFileSystemAccess
    {
        private readonly IDictionary<string, List<BaseFileSystemEntry>> _entries = new Dictionary<string, List<BaseFileSystemEntry>>();

        public FakeFileSystemAccess WithFiles(string rootPath, double fileSize, params string[] fileNames)
        {
            var newEntries = fileNames
                .Select(filename => new FakeFileSystemEntry(Path.Join(rootPath, filename), FileSystemEntryType.File, fileSize));
            AppendEntries(rootPath, newEntries);
            return this;
        }

        public FakeFileSystemAccess WithDirectories(string rootPath, double dirSize, params string[] directoryNames)
        {
            var newEntries = directoryNames
                .Select(filename => new FakeFileSystemEntry(Path.Join(rootPath, filename), FileSystemEntryType.Directory, dirSize));
            AppendEntries(rootPath, newEntries);
            return this;
        }

        public IEnumerable<BaseFileSystemEntry> GetEntries(string directoryPath)
        {
            return _entries[Normalize(directoryPath)];
        }

        private void AppendEntries(string directoryPath, IEnumerable<BaseFileSystemEntry> entries)
        {
            var matchingKey = _entries.Keys.SingleOrDefault(key => Normalize(key) == Normalize(directoryPath));

            if (matchingKey is null)
                _entries[directoryPath] = entries.ToList();
            else
                _entries[directoryPath].AddRange(entries);
        }

        private static string Normalize(string key) => key.Replace('\\', '/');
    }
}
