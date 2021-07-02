using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class TextureManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IRenderingBackend _renderingBackend;
        private readonly IFileSystem _fileSystem;

        public TextureManagedAssetFactory(IRenderingBackend renderingBackend, IFileSystem fileSystem)
        {
            _renderingBackend = renderingBackend;
            _fileSystem = fileSystem;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(ITexture))
            {
                var managedAsset = new TextureManagedAsset(assetInfo, _renderingBackend.Renderer2D, _fileSystem);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}