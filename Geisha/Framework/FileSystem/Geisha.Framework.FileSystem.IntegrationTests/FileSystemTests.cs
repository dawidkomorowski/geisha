using NUnit.Framework;

namespace Geisha.Framework.FileSystem.IntegrationTests
{
    [TestFixture]
    public class FileSystemTests
    {
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new FileSystem();
        }
    }
}