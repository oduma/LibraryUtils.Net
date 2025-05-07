using System;
using System.IO;
using System.Linq;
using Xunit;
using Sciendo.Junk.Detect.Library;

namespace Sciendo.Junk.Detect.Library.Tests
{
    public class JunkDetectorTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly IJunkDetector _junkDetector;

        public JunkDetectorTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "JunkDetectorTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _junkDetector = new JunkDetector();
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void Detect_EmptyPath_ReturnsEmptyCollection()
        {
            var result = _junkDetector.Detect("", new[] { ".txt" });
            Assert.Empty(result);
        }

        [Fact]
        public void Detect_NonExistentPath_ReturnsEmptyCollection()
        {
            var result = _junkDetector.Detect(Path.Combine(_testDirectory, "NonExistent"), new[] { ".txt" });
            Assert.Empty(result);
        }

        [Fact]
        public void Detect_WithMatchingExtensions_ReturnsNoFiles()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(filePath, "test content");

            // Act
            var result = _junkDetector.Detect(_testDirectory, new[] { ".txt" });

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Detect_WithNonMatchingExtensions_ReturnsFiles()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(filePath, "test content");

            // Act
            var result = _junkDetector.Detect(_testDirectory, new[] { ".doc" }).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(filePath, result[0]);
        }

        [Fact]
        public void Detect_WithSubdirectories_ReturnsAllMatchingFiles()
        {
            // Arrange
            var subDir = Path.Combine(_testDirectory, "subdir");
            Directory.CreateDirectory(subDir);
            var file1 = Path.Combine(_testDirectory, "test1.txt");
            var file2 = Path.Combine(subDir, "test2.txt");
            File.WriteAllText(file1, "test content 1");
            File.WriteAllText(file2, "test content 2");

            // Act
            var result = _junkDetector.Detect(_testDirectory, new[] { ".doc" }).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(file1, result);
            Assert.Contains(file2, result);
        }

        [Fact]
        public void GetExtensions_EmptyPath_ReturnsEmptyCollection()
        {
            var result = _junkDetector.GetExtensions("");
            Assert.Empty(result);
        }

        [Fact]
        public void GetExtensions_NonExistentPath_ReturnsEmptyCollection()
        {
            var result = _junkDetector.GetExtensions(Path.Combine(_testDirectory, "NonExistent"));
            Assert.Empty(result);
        }

        [Fact]
        public void GetExtensions_WithFiles_ReturnsUniqueExtensions()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_testDirectory, "test1.txt"), "content");
            File.WriteAllText(Path.Combine(_testDirectory, "test2.txt"), "content");
            File.WriteAllText(Path.Combine(_testDirectory, "test.doc"), "content");

            // Act
            var result = _junkDetector.GetExtensions(_testDirectory).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(".txt", result);
            Assert.Contains(".doc", result);
        }

        [Fact]
        public void GetExtensions_WithSubdirectories_ReturnsAllUniqueExtensions()
        {
            // Arrange
            var subDir = Path.Combine(_testDirectory, "subdir");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(_testDirectory, "test1.txt"), "content");
            File.WriteAllText(Path.Combine(subDir, "test2.doc"), "content");
            File.WriteAllText(Path.Combine(subDir, "test3.pdf"), "content");

            // Act
            var result = _junkDetector.GetExtensions(_testDirectory).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(".txt", result);
            Assert.Contains(".doc", result);
            Assert.Contains(".pdf", result);
        }
    }
}
