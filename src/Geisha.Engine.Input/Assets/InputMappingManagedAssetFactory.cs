using Geisha.Common;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Input.Assets
{
    internal sealed class InputMappingManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public InputMappingManagedAssetFactory(IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(InputMapping))
            {
                var managedAsset = new InputMappingManagedAsset(assetInfo, _fileSystem, _jsonSerializer);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}