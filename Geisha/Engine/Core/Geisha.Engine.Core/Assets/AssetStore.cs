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
        ///     Returns <see cref="AssetInfo" /> of each registered asset.
        /// </summary>
        /// <returns><see cref="AssetInfo" /> of each registered asset.</returns>
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
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<IAssetDiscoveryRule> _assetDiscoveryRules;
        private readonly IEnumerable<IAssetFactory> _assetFactories;

        private readonly Dictionary<AssetId, IAsset> _assets = new Dictionary<AssetId, IAsset>();
        private readonly Dictionary<object, AssetId> _assetsIds = new Dictionary<object, AssetId>();

        public AssetStore(IFileSystem fileSystem, IEnumerable<IAssetDiscoveryRule> assetDiscoveryRules,
            IEnumerable<IAssetFactory> assetFactories)
        {
            _fileSystem = fileSystem;
            _assetDiscoveryRules = assetDiscoveryRules;
            _assetFactories = assetFactories;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns asset of given type and id.
        /// </summary>
        public TAsset GetAsset<TAsset>(AssetId assetId)
        {
            if (!_assets.TryGetValue(assetId, out var asset)) throw new AssetNotRegisteredException(assetId, typeof(TAsset));
            if (asset.AssetInfo.AssetType != typeof(TAsset)) throw new AssetNotRegisteredException(assetId, typeof(TAsset));

            if (!asset.IsLoaded)
            {
                Log.Info($"Asset not yet loaded, will be loaded now. Asset info: {asset.AssetInfo}");
                asset.Load();
                _assetsIds.Add(asset.AssetInstance, assetId);
            }

            return (TAsset) asset.AssetInstance;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns id of given asset instance.
        /// </summary>
        public AssetId GetAssetId(object asset)
        {
            if (!_assetsIds.TryGetValue(asset, out var assetId))
                throw new ArgumentException("Given asset was not loaded by this asset store.", nameof(asset));

            return assetId;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns <see cref="AssetInfo" /> of each registered asset.
        /// </summary>
        public IEnumerable<AssetInfo> GetRegisteredAssets()
        {
            return _assets.Values.Select(a => a.AssetInfo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Registers an asset for access in asset store.
        /// </summary>
        public void RegisterAsset(AssetInfo assetInfo)
        {
            if (_assets.ContainsKey(assetInfo.AssetId))
            {
                var asset = _assets[assetInfo.AssetId];
                Log.Warn(
                    $"Asset already registered, will be unloaded and overridden. All existing references may become invalid. Existing asset info: {asset.AssetInfo}. New asset info: {assetInfo}");

                if (asset.IsLoaded)
                {
                    _assetsIds.Remove(asset.AssetInstance);
                    asset.Unload();
                }
            }

            var singleOrEmptyAsset = _assetFactories.SelectMany(f => f.Create(assetInfo, this)).ToList();

            if (singleOrEmptyAsset.Count == 0) throw new AssetFactoryNotFoundException(assetInfo);
            if (singleOrEmptyAsset.Count > 1) throw new MultipleAssetFactoriesFoundException(assetInfo);

            _assets[assetInfo.AssetId] = singleOrEmptyAsset.Single();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Registers all assets discovered in specified directory path.
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