using NUnit.Framework;

namespace Geisha.Framework.FileSystem.IntegrationTests
{
    [TestFixture]
    public class FileSystemTests : IntegrationTests<IFileSystem>
    {
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new FileSystem();
        }


    }
}