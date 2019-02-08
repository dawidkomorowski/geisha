using System.Collections.Generic;
using System.Linq;
using Geisha.Common;
using Geisha.Engine.Core.Configuration;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Assets
{
    internal class AssetsDiscoveryEngine
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<IAssetDiscoveryRule> _assetDiscoveryRules;

        public AssetsDiscoveryEngine(IConfigurationManager configurationManager, IFileSystem fileSystem, IEnumerable<IAssetDiscoveryRule> assetDiscoveryRules)
        {
            _configurationManager = configurationManager;
            _fileSystem = fileSystem;
            _assetDiscoveryRules = assetDiscoveryRules;
        }

        public IEnumerable<AssetInfo> DiscoverAssets()
        {
            var assetsRootDirectoryPath = _configurationManager.GetConfiguration<CoreConfiguration>().AssetsRootDirectoryPath;
            var rootDirectory = _fileSystem.GetDirectory(assetsRootDirectoryPath);

            return rootDirectory.Files.SelectMany(f => _assetDiscoveryRules.SelectMany(r => r.Discover(f)));
        }
    }

    public interface IAssetDiscoveryRule
    {
        ISingleOrEmpty<AssetInfo> Discover(IFile file);
    }
}