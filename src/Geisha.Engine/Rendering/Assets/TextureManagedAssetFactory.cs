using Geisha.Common;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class TextureManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IRenderer2D _renderer2D;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public TextureManagedAssetFactory(IRenderer2D renderer2D, IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _renderer2D = renderer2D;
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(ITexture))
            {
                var managedAsset = new TextureManagedAsset(assetInfo, _renderer2D, _fileSystem, _jsonSerializer);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}