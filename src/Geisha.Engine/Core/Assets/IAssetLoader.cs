using System;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Defines interface of a loader of specific asset type. The loader is responsible for loading the asset from file
    ///     into memory and unloading it from memory.
    /// </summary>
    /// <remarks>
    ///     Implement this interface to provide the engine a capability of loading specific type of asset. In order to
    ///     allow the engine to use the implementation it must be register in a container.
    /// </remarks>
    public interface IAssetLoader
    {
        /// <summary>
        ///     <see cref="AssetType" /> of asset this loader is capable of loading and unloading.
        /// </summary>
        AssetType AssetType { get; }

        /// <summary>
        ///     <see cref="Type" /> of asset class this loader is capable of loading and unloading.
        /// </summary>
        Type AssetClassType { get; }

        /// <summary>
        ///     Loads an asset specified by <paramref name="assetInfo" />.
        /// </summary>
        /// <param name="assetInfo">Specifies an asset to load.</param>
        /// <param name="assetStore">Provides ability to access assets that asset being loaded depends on.</param>
        /// <returns>
        ///     Instance of asset class representing loaded asset. The <see cref="Type" /> of asset class should match
        ///     <see cref="AssetClassType" />.
        /// </returns>
        object LoadAsset(AssetInfo assetInfo, IAssetStore assetStore);

        /// <summary>
        ///     Unloads specified <paramref name="asset" /> from memory. The actual behavior is implementation specific as managed
        ///     memory is subject for garbage collection. However unloaded asset should no longer be used.
        /// </summary>
        /// <param name="asset">Asset object to be unloaded.</param>
        void UnloadAsset(object asset);
    }
}