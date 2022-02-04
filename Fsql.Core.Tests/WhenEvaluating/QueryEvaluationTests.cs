using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Fsql.Core.Evaluation;
using Fsql.Core.FileSystem;
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
                .WithFiles(givenPath);

            var result = Evaluate(givenPath, fsAccess);
            result.Rows.Should().BeEmpty();
        }

        [Fact]
        public void Given1FileAnd1DirectoryReturn2Entries()
        {
            var givenPath = "./path";
            var fsAccess = new FakeFileSystemAccess()
                .WithFiles(givenPath, "file01.txt")
                .WithDirectories(givenPath, "sub_directory");

            var result = Evaluate(givenPath, fsAccess);
            result.Rows.Should().HaveCount(2);
        }

        [Fact]
        public void Given3FilesReturn3Entries()
        {
            var givenPath = "./EXAMPLE_PATH";
            var fsAccess = new FakeFileSystemAccess()
                .WithFiles(givenPath, "00", "11", "22")
                .WithFiles("./other-path", "0a", "1b", "2c", "3d", "4e");

            var result = Evaluate(givenPath, fsAccess);
            result.Rows.Should().HaveCount(3);
        }

        [Fact]
        public void Given3DirectoriesReturn3Entries()
        {
            var givenPath = "./EXAMPLE_PATH";
            var fsAccess = new FakeFileSystemAccess()
                .WithDirectories(givenPath, "00", "11", "22")
                .WithDirectories("./other-path", "0a", "1b", "2c", "3d", "4e");

            var result = Evaluate(givenPath, fsAccess);
            result.Rows.Should().HaveCount(3);
        }

        private QueryEvaluationResult Evaluate(string givenPath, IFileSystemAccess fileSystemAccess)
        {
            var givenQuery = new Query(new List<string>(), givenPath, OrderByExpression.NoOrdering);
            var sut = new QueryEvaluation(fileSystemAccess);
            return sut.Evaluate(givenQuery);
        }
    }

    internal record FakeFileSystemEntry : BaseFileSystemEntry
    {
        public override double Size { get; }
        public override string AbsolutePath => $"/absolute/path/{FullPath}";
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

        public FakeFileSystemAccess WithFiles(string rootPath, params string[] fileNames)
        {
            var newEntries = fileNames
                .Select(filename => new FakeFileSystemEntry(Path.Join(rootPath, filename), FileSystemEntryType.File, 1234));
            AppendEntries(rootPath, newEntries);
            return this;
        }

        public FakeFileSystemAccess WithDirectories(string rootPath, params string[] directoryNames)
        {
            var newEntries = directoryNames
                .Select(filename => new FakeFileSystemEntry(Path.Join(rootPath, filename), FileSystemEntryType.Directory, 0));
            AppendEntries(rootPath, newEntries);
            return this;
        }

        public IEnumerable<BaseFileSystemEntry> GetEntries(string directoryPath)
        {
            return _entries[directoryPath];
        }

        private void AppendEntries(string directoryPath, IEnumerable<BaseFileSystemEntry> entries)
        {
            if (_entries.ContainsKey(directoryPath))
                _entries[directoryPath].AddRange(entries);
            else
                _entries[directoryPath] = entries.ToList();
        }
    }
}
