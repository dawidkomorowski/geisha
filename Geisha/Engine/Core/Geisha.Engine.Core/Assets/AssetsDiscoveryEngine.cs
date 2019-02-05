using Geisha.Engine.Core.Configuration;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Assets
{
    internal class AssetsDiscoveryEngine
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IFileSystem _fileSystem;

        public AssetsDiscoveryEngine(IConfigurationManager configurationManager, IFileSystem fileSystem)
        {
            _configurationManager = configurationManager;
            _fileSystem = fileSystem;
        }

        public void DiscoverAssets()
        {
            var assetsRootDirectoryPath = _configurationManager.GetConfiguration<CoreConfiguration>().AssetsRootDirectoryPath;
            _fileSystem.GetDirectory(assetsRootDirectoryPath);
        }
    }
}