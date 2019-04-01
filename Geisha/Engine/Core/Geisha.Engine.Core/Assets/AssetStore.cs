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

    /// <summary>
    ///     The exception that is thrown when accessing an asset that is not registered in <see cref="AssetStore" />.
    /// </summary>
    public sealed class AssetNotRegisteredException : Exception
    {
        public AssetNotRegisteredException(AssetId assetId, Type assetType) : base(
            $"Asset of type {assetType.FullName} with id {assetId} was not registered in an asset store.")
        {
            AssetId = assetId;
            AssetType = assetType;
        }

        /// <summary>
        ///     Asset id of asset that access to has failed.
        /// </summary>
        public AssetId AssetId { get; }

        /// <summary>
        ///     Type of asset that access to has failed.
        /// </summary>
        public Type AssetType { get; }
    }

    /// <summary>
    ///     The exception that is thrown when registering an asset in <see cref="AssetStore" /> for which no implementation of
    ///     <see cref="IManagedAssetFactory" /> is provided.
    /// </summary>
    public sealed class AssetFactoryNotFoundException : Exception
    {
        public AssetFactoryNotFoundException(AssetInfo assetInfo) : base(
            $"No asset factory found for asset info: {assetInfo}. Single implementation of {nameof(IManagedAssetFactory)} per asset type is required.")
        {
            AssetInfo = assetInfo;
        }

        /// <summary>
        ///     Asset info of asset that registration has failed.
        /// </summary>
        public AssetInfo AssetInfo { get; }
    }

    /// <summary>
    ///     The exception that is thrown when registering an asset in <see cref="AssetStore" /> for which multiple
    ///     implementations of <see cref="IManagedAssetFactory" /> are provided.
    /// </summary>
    public sealed class MultipleAssetFactoriesFoundException : Exception
    {
        public MultipleAssetFactoriesFoundException(AssetInfo assetInfo) : base(
            $"Multiple asset factories found for asset info: {assetInfo}. Single implementation of {nameof(IManagedAssetFactory)} per asset type is required.")
        {
            AssetInfo = assetInfo;
        }

        /// <summary>
        ///     Asset info of asset that registration has failed.
        /// </summary>
        public AssetInfo AssetInfo { get; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides access to assets.
    /// </summary>
    internal sealed class AssetStore : IAssetStore
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AssetStore));
        private readonly IEnumerable<IAssetDiscoveryRule> _assetDiscoveryRules;
        private readonly Dictionary<object, AssetId> _assetsIds = new Dictionary<object, AssetId>();
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<IManagedAssetFactory> _managedAssetFactories;
        private readonly Dictionary<AssetId, IManagedAsset> _managedAssets = new Dictionary<AssetId, IManagedAsset>();

        public AssetStore(IFileSystem fileSystem, IEnumerable<IAssetDiscoveryRule> assetDiscoveryRules,
            IEnumerable<IManagedAssetFactory> managedAssetFactories)
        {
            _fileSystem = fileSystem;
            _assetDiscoveryRules = assetDiscoveryRules;
            _managedAssetFactories = managedAssetFactories;

            Log.Debug("Discovering asset discovery rules...");
            foreach (var assetDiscoveryRule in _assetDiscoveryRules)
            {
                Log.Debug($"Asset discovery rule found: {assetDiscoveryRule.GetType().FullName}.");
            }

            Log.Debug("Asset discovery rules discovery completed.");

            Log.Debug("Discovering managed asset factories...");
            foreach (var managedAssetFactory in _managedAssetFactories)
            {
                Log.Debug($"Managed asset factory found: {managedAssetFactory.GetType().FullName}.");
            }

            Log.Debug("Managed asset factories discovery completed.");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns asset of given type and id.
        /// </summary>
        public TAsset GetAsset<TAsset>(AssetId assetId)
        {
            if (!_managedAssets.TryGetValue(assetId, out var managedAsset)) throw new AssetNotRegisteredException(assetId, typeof(TAsset));
            if (managedAsset.AssetInfo.AssetType != typeof(TAsset)) throw new AssetNotRegisteredException(assetId, typeof(TAsset));

            if (!managedAsset.IsLoaded)
            {
                Log.Debug($"Asset not yet loaded, will be loaded now. Asset info: {managedAsset.AssetInfo}");
                managedAsset.Load();
                _assetsIds.Add(managedAsset.AssetInstance, assetId);
            }

            return (TAsset) managedAsset.AssetInstance;
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
            return _managedAssets.Values.Select(a => a.AssetInfo);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Registers an asset for access in asset store.
        /// </summary>
        public void RegisterAsset(AssetInfo assetInfo)
        {
            if (_managedAssets.ContainsKey(assetInfo.AssetId))
            {
                var managedAsset = _managedAssets[assetInfo.AssetId];
                Log.Warn(
                    $"Asset already registered, will be unloaded and overridden. All existing references may become invalid. Existing asset info: {managedAsset.AssetInfo}. New asset info: {assetInfo}");

                if (managedAsset.IsLoaded)
                {
                    _assetsIds.Remove(managedAsset.AssetInstance);
                    managedAsset.Unload();
                }
            }

            var singleOrEmptyManagedAsset = _managedAssetFactories.SelectMany(f => f.Create(assetInfo, this)).ToList();

            if (singleOrEmptyManagedAsset.Count == 0) throw new AssetFactoryNotFoundException(assetInfo);
            if (singleOrEmptyManagedAsset.Count > 1) throw new MultipleAssetFactoriesFoundException(assetInfo);

            _managedAssets[assetInfo.AssetId] = singleOrEmptyManagedAsset.Single();

            Log.Debug($"Asset registered: {assetInfo}.");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Registers all assets discovered in specified directory path.
        /// </summary>
        public void RegisterAssets(string assetDiscoveryPath)
        {
            Log.Debug($"Registering assets from directory: {assetDiscoveryPath} ...");

            var rootDirectory = _fileSystem.GetDirectory(assetDiscoveryPath);
            var discoveredAssetInfos = GetAllFilesInDirectoryTree(rootDirectory).SelectMany(f => _assetDiscoveryRules.SelectMany(r => r.Discover(f)));

            foreach (var assetInfo in discoveredAssetInfos)
            {
                RegisterAsset(assetInfo);
            }

            Log.Debug($"Assets registration completed.");
        }

        private static IEnumerable<IFile> GetAllFilesInDirectoryTree(IDirectory directory)
        {
            return directory.Files.Concat(directory.Directories.SelectMany(GetAllFilesInDirectoryTree));
        }
    }
}