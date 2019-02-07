using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<AssetInfo> DiscoverAssets()
        {
            var assetsRootDirectoryPath = _configurationManager.GetConfiguration<CoreConfiguration>().AssetsRootDirectoryPath;
            var rootDirectory = _fileSystem.GetDirectory(assetsRootDirectoryPath);

            return rootDirectory.Files.Select(f => new AssetInfo(new AssetId(Guid.NewGuid()), null, null));
        }
    }
}