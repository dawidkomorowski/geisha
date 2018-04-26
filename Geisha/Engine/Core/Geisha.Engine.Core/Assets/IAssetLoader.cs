using System;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Provides functionality to load asset from file.
    /// </summary>
    /// <remarks>
    ///     For each asset type the implementation of this interface must be provided. There can be only single
    ///     implementation per asset type.
    /// </remarks>
    public interface IAssetLoader
    {
        /// <summary>
        ///     Type of asset supported by loader.
        /// </summary>
        Type AssetType { get; }

        /// <summary>
        ///     Loads asset from file.
        /// </summary>
        /// <param name="filePath">Path to asset file.</param>
        /// <returns>Instance of loaded asset.</returns>
        object Load(string filePath);
    }
}