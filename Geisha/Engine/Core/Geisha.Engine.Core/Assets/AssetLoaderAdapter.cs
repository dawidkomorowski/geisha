using System;

namespace Geisha.Engine.Core.Assets
{
    /// <inheritdoc />
    /// <summary>
    ///     Adapter class simplifying implementation of <see cref="T:Geisha.Engine.Core.Assets.IAssetLoader" />.
    /// </summary>
    /// <typeparam name="TAsset">Asset type the loader supports.</typeparam>
    public abstract class AssetLoaderAdapter<TAsset> : IAssetLoader
    {
        public Type AssetType => typeof(TAsset);
        public abstract object Load(string filePath);
    }
}