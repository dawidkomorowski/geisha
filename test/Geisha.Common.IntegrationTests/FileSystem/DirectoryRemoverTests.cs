using System.IO;
using Geisha.Common.FileSystem;
using NUnit.Framework;

namespace Geisha.Common.IntegrationTests.FileSystem
{
    [TestFixture]
    public class DirectoryRemoverTests
    {
        [Test]
        public void RemoveDirectoryRecursively_ShouldRemoveDirectoryTogetherWithAllItsContent()
        {
            // Arrange
            var rootDirectory = TestUtils.Utils.GetPathUnderTestDirectory("DirectoryRemoverTestsRootDirectory");
            Assume.That(Directory.Exists(rootDirectory), Is.False);

            Directory.CreateDirectory(Path.Combine(rootDirectory, "Level1", "Level2"));
            Assume.That(Directory.Exists(Path.Combine(rootDirectory, "Level1", "Level2")), Is.True);

            File.WriteAllText(Path.Combine(rootDirectory, "FileAtLevel0"), "Level 0 Content");
            File.WriteAllText(Path.Combine(rootDirectory, "Level1", "FileAtLevel1"), "Level 1 Content");
            File.WriteAllText(Path.Combine(rootDirectory, "Level1", "Level2", "FileAtLevel2"), "Level 2 Content");

            Assume.That(File.Exists(Path.Combine(rootDirectory, "FileAtLevel0")), Is.True);
            Assume.That(File.Exists(Path.Combine(rootDirectory, "Level1", "FileAtLevel1")), Is.True);
            Assume.That(File.Exists(Path.Combine(rootDirectory, "Level1", "Level2", "FileAtLevel2")), Is.True);

            // Act
            DirectoryRemover.RemoveDirectoryRecursively(rootDirectory);

            // Assert
            Assert.That(Directory.Exists(rootDirectory), Is.False);
        }
    }
}