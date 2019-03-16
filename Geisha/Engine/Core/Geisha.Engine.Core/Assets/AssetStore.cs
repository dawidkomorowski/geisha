using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Provides access to assets.
    /// </summary>
    public interface IAssetStore
    {
        /// <summary>
        ///     Returns asset of given type and id.
        /// </summary>
        /// <typeparam name="TAsset">Type of asset requested.</typeparam>
        /// <param name="assetId">Id of asset requested.</param>
        /// <returns>Instance of requested asset.</returns>
        /// <remarks>
        ///     An asset requires registration in asset store first to be available for access. To register an asset use
        ///     <see cref="RegisterAsset" />. Asset store manages loading assets into memory by itself. If requested asset is not
        ///     yet loaded into memory it is loaded then and its instance returned. If asset was already loaded into memory and is
        ///     available in cache of asset store then its instance is immediately served from cache.
        /// </remarks>
        TAsset GetAsset<TAsset>(AssetId assetId);

        /// <summary>
        ///     Returns id of given asset instance.
        /// </summary>
        /// <param name="asset">Asset for which id is requested.</param>
        /// <returns>Asset id.</returns>
        /// <remarks>Asset id can be provided only for those assets that were loaded with this instance of asset store.</remarks>
        AssetId GetAssetId(object asset);

        /// <summary>
        ///     Returns all registered <see cref="AssetInfo" />.
        /// </summary>
        /// <returns>All registered <see cref="AssetInfo" />.</returns>
        IEnumerable<AssetInfo> GetRegisteredAssets();

        /// <summary>
        ///     Registers an asset for access in asset store.
        /// </summary>
        /// <param name="assetInfo">Asset registration info.</param>
        void RegisterAsset(AssetInfo assetInfo);

        /// <summary>
        ///     Registers all assets discovered in specified directory path.
        /// </summary>
        /// <param name="assetDiscoveryPath">Root directory path for assets discovery and registration process.</param>
        void RegisterAssets(string assetDiscoveryPath);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides access to assets.
    /// </summary>
    internal sealed class AssetStore : IAssetStore
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AssetStore));
        private readonly IEnumerable<IAssetDiscoveryRule> _assetDiscoveryRules;
        private readonly Dictionary<object, AssetId> _assetIds = new Dictionary<object, AssetId>();
        private readonly IAssetLoaderProvider _assetLoaderProvider;
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<AssetInfo, object> _loadedAssets = new Dictionary<AssetInfo, object>();
        private readonly Dictionary<Tuple<Type, AssetId>, AssetInfo> _registeredAssets = new Dictionary<Tuple<Type, AssetId>, AssetInfo>();

        public AssetStore(IAssetLoaderProvider assetLoaderProvider, IFileSystem fileSystem, IEnumerable<IAssetDiscoveryRule> assetDiscoveryRules)
        {
            _assetLoaderProvider = assetLoaderProvider;
            _fileSystem = fileSystem;
            _assetDiscoveryRules = assetDiscoveryRules;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns asset of given type and id.
        /// </summary>
        public TAsset GetAsset<TAsset>(AssetId assetId)
        {
            if (!_registeredAssets.TryGetValue(Tuple.Create(typeof(TAsset), assetId), out var assetInfo))
                throw new GeishaEngineException($"Asset not found for type {typeof(TAsset).FullName} and {assetId}.");

            if (!_loadedAssets.TryGetValue(assetInfo, out var asset))
            {
                Log.Info($"Asset not yet loaded, will be loaded now. Asset info: {assetInfo}");

                var assetLoader = _assetLoaderProvider.GetLoaderFor(assetInfo.AssetType);
                asset = assetLoader.Load(assetInfo.AssetFilePath);

                _loadedAssets.Add(assetInfo, asset);
                _assetIds.Add(asset, assetInfo.AssetId);
            }

            return (TAsset) asset;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns id of given asset instance.
        /// </summary>
        public AssetId GetAssetId(object asset)
        {
            if (!_assetIds.TryGetValue(asset, out var assetId))
                throw new ArgumentException("Given asset was not loaded by this asset store.", nameof(asset));

            return assetId;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns all registered <see cref="T:Geisha.Engine.Core.Assets.AssetInfo" />.
        /// </summary>
        public IEnumerable<AssetInfo> GetRegisteredAssets()
        {
            return _registeredAssets.Values;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Registers an asset for access in asset store.
        /// </summary>
        public void RegisterAsset(AssetInfo assetInfo)
        {
            var key = Tuple.Create(assetInfo.AssetType, assetInfo.AssetId);

            if (_registeredAssets.ContainsKey(key))
            {
                Log.Warn(
                    $"Asset already registered, will be overridden. Existing asset info: {_registeredAssets[key]}. New asset info: {assetInfo}");
            }

            _registeredAssets[key] = assetInfo;
        }

        /// <inheritdoc />
        /// <summary>
        /// Registers all assets discovered in specified directory path.
        /// </summary>
        public void RegisterAssets(string assetDiscoveryPath)
        {
            var rootDirectory = _fileSystem.GetDirectory(assetDiscoveryPath);
            var discoveredAssetInfos = GetAllFilesInDirectoryTree(rootDirectory).SelectMany(f => _assetDiscoveryRules.SelectMany(r => r.Discover(f)));

            foreach (var assetInfo in discoveredAssetInfos)
            {
                RegisterAsset(assetInfo);
            }
        }

        private static IEnumerable<IFile> GetAllFilesInDirectoryTree(IDirectory directory)
        {
            return directory.Files.Concat(directory.Directories.SelectMany(GetAllFilesInDirectoryTree));
        }
    }
}