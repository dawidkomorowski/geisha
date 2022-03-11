using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Logging;

namespace Geisha.Engine.Core.Assets
{
    // TODO Maybe all asset classes should implement common interface or inherit common base class to better define what is an asset.
    // TODO Now any type can be an asset in a bit vague way (see GetAssetId method necessary as asset itself does not know its id).
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
        ///     An asset requires registration in asset store first to be available for access. Engine discovers and registers
        ///     assets automatically at startup. To configure directory path where engine searches for assets use
        ///     <see cref="CoreConfiguration.AssetsRootDirectoryPath" /> in <c>engine-config.json</c> file. To manually register an
        ///     asset use <see cref="RegisterAsset" /> or <see cref="RegisterAssets" />. Asset store manages loading assets into
        ///     memory by itself. If requested asset is not yet loaded into memory it is loaded then and its instance returned. If
        ///     asset was already loaded into memory and is available then its instance is immediately served.
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
        /// <param name="directoryPath">Root directory path for assets discovery and registration process.</param>
        void RegisterAssets(string directoryPath);

        /// <summary>
        ///     Unloads asset with specified id.
        /// </summary>
        /// <param name="assetId">Id of asset to unload.</param>
        /// <remarks>
        ///     If asset is loaded then <see cref="UnloadAsset" /> unloads this asset. If asset is not loaded then
        ///     <see cref="UnloadAsset" /> does nothing.
        /// </remarks>
        void UnloadAsset(AssetId assetId);

        /// <summary>
        ///     Unloads all loaded assets that was registered in asset store.
        /// </summary>
        void UnloadAssets();
    }

    /// <summary>
    ///     The exception that is thrown when accessing an asset that is not registered in <see cref="IAssetStore" />.
    /// </summary>
    public sealed class AssetNotRegisteredException : Exception
    {
        public AssetNotRegisteredException(AssetId assetId) : base(
            $"Asset with id {assetId} was not registered in an asset store.")
        {
            AssetId = assetId;
        }

        public AssetNotRegisteredException(AssetId assetId, Type assetType) : base(
            $"Asset of class type {assetType.FullName} with id {assetId} was not registered in an asset store.")
        {
            AssetId = assetId;
            AssetType = assetType;
        }

        /// <summary>
        ///     Asset id of asset that access to has failed.
        /// </summary>
        public AssetId AssetId { get; }

        /// <summary>
        ///     Type of asset that access to has failed. Can be <c>null</c> when type is unknown.
        /// </summary>
        public Type? AssetType { get; }
    }

    /// <summary>
    ///     The exception that is thrown when registering an asset in <see cref="IAssetStore" /> for which no implementation of
    ///     <see cref="IAssetLoader" /> is provided.
    /// </summary>
    public sealed class AssetLoaderNotFoundException : Exception
    {
        public AssetLoaderNotFoundException(AssetInfo assetInfo) : base(
            $"No asset loader found for asset info: {assetInfo}. Single implementation of {nameof(IAssetLoader)} per asset type is required.")
        {
            AssetInfo = assetInfo;
        }

        /// <summary>
        ///     Asset info of asset for which no implementation of <see cref="IAssetLoader" /> was found.
        /// </summary>
        public AssetInfo AssetInfo { get; }
    }

    internal sealed class AssetStore : IAssetStore, IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AssetStore));
        private readonly Dictionary<AssetType, IAssetLoader> _assetLoaders;
        private readonly Dictionary<object, AssetId> _assetsIds = new Dictionary<object, AssetId>();
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<AssetId, RegisteredAsset> _registeredAssets = new Dictionary<AssetId, RegisteredAsset>();

        public AssetStore(IFileSystem fileSystem, IEnumerable<IAssetLoader> assetLoaders)
        {
            _fileSystem = fileSystem;

            var assetLoadersArray = assetLoaders as IAssetLoader[] ?? assetLoaders.ToArray();
            var multipleAssetLoadersForSingleAssetType = assetLoadersArray.GroupBy(al => al.AssetType).FirstOrDefault(g => g.Count() > 1);
            if (multipleAssetLoadersForSingleAssetType != null)
                throw new ArgumentException($"Multiple asset loaders for the same asset type {multipleAssetLoadersForSingleAssetType.Key} are not allowed.");

            _assetLoaders = assetLoadersArray.ToDictionary(al => al.AssetType);

            Log.Debug("Available asset loaders:");
            foreach (var assetLoader in _assetLoaders.Values)
            {
                Log.Debug(assetLoader.GetType().FullName);
            }
        }

        public TAsset GetAsset<TAsset>(AssetId assetId)
        {
            if (!_registeredAssets.TryGetValue(assetId, out var registeredAsset)) throw new AssetNotRegisteredException(assetId, typeof(TAsset));
            if (registeredAsset.AssetClassType != typeof(TAsset)) throw new AssetNotRegisteredException(assetId, typeof(TAsset));

            if (!registeredAsset.IsLoaded)
            {
                Log.Debug($"Asset not yet loaded, will be loaded now. Asset info: {registeredAsset.AssetInfo}");
                registeredAsset.Load();
                Debug.Assert(registeredAsset.AssetInstance != null, "registeredAsset.AssetInstance != null");
                _assetsIds.Add(registeredAsset.AssetInstance, assetId);
            }

            Debug.Assert(registeredAsset.AssetInstance != null, "registeredAsset.AssetInstance != null");
            return (TAsset)registeredAsset.AssetInstance;
        }

        public AssetId GetAssetId(object asset)
        {
            if (!_assetsIds.TryGetValue(asset, out var assetId)) throw new ArgumentException("Given asset was not loaded by this asset store.", nameof(asset));

            return assetId;
        }

        public IEnumerable<AssetInfo> GetRegisteredAssets()
        {
            return _registeredAssets.Values.Select(ra => ra.AssetInfo);
        }

        public void RegisterAsset(AssetInfo assetInfo)
        {
            if (_registeredAssets.ContainsKey(assetInfo.AssetId))
            {
                var registeredAsset = _registeredAssets[assetInfo.AssetId];
                Log.Warn(
                    $"Asset already registered, will be unloaded and overridden. All existing references may become invalid. Existing asset info: {registeredAsset.AssetInfo}. New asset info: {assetInfo}");

                if (registeredAsset.IsLoaded)
                {
                    Debug.Assert(registeredAsset.AssetInstance != null, "registeredAsset.AssetInstance != null");
                    _assetsIds.Remove(registeredAsset.AssetInstance);
                    registeredAsset.Unload();
                }
            }

            if (!_assetLoaders.TryGetValue(assetInfo.AssetType, out var assetLoader)) throw new AssetLoaderNotFoundException(assetInfo);

            _registeredAssets[assetInfo.AssetId] = new RegisteredAsset(assetInfo, assetLoader, this);
            Log.Debug($"Asset registered: {assetInfo}.");
        }

        public void RegisterAssets(string directoryPath)
        {
            Log.Debug($"Registering assets from directory: {directoryPath}");

            var rootDirectory = _fileSystem.GetDirectory(directoryPath);
            var discoveredAssetInfos = GetAllFilesInDirectoryTree(rootDirectory).SelectMany(TryGetAssetInfoFromFile);

            foreach (var assetInfo in discoveredAssetInfos)
            {
                RegisterAsset(assetInfo);
            }

            Log.Debug("Assets registration completed.");
        }

        public void UnloadAsset(AssetId assetId)
        {
            if (!_registeredAssets.TryGetValue(assetId, out var registeredAsset)) throw new AssetNotRegisteredException(assetId);

            if (registeredAsset.IsLoaded)
            {
                Debug.Assert(registeredAsset.AssetInstance != null, "registeredAsset.AssetInstance != null");
                _assetsIds.Remove(registeredAsset.AssetInstance);
                registeredAsset.Unload();
            }
            else
            {
                Log.Debug($"Asset is not loaded. Skipping asset unload. Asset info: {registeredAsset.AssetInfo}");
            }
        }

        public void UnloadAssets()
        {
            foreach (var assetId in _registeredAssets.Keys)
            {
                UnloadAsset(assetId);
            }
        }

        public void Dispose()
        {
            UnloadAssets();
        }

        private static IEnumerable<IFile> GetAllFilesInDirectoryTree(IDirectory directory) =>
            directory.Files.Concat(directory.Directories.SelectMany(GetAllFilesInDirectoryTree));

        private static ISingleOrEmpty<AssetInfo> TryGetAssetInfoFromFile(IFile file)
        {
            if (!AssetFileUtils.IsAssetFile(file.Path)) return SingleOrEmpty.Empty<AssetInfo>();

            using var fileStream = file.OpenRead();
            var assetData = AssetData.Load(fileStream);
            var assetInfo = new AssetInfo(assetData.AssetId, assetData.AssetType, file.Path);
            return SingleOrEmpty.Single(assetInfo);
        }

        private sealed class RegisteredAsset
        {
            private readonly IAssetLoader _assetLoader;
            private readonly IAssetStore _assetStore;

            public RegisteredAsset(AssetInfo assetInfo, IAssetLoader assetLoader, IAssetStore assetStore)
            {
                AssetInfo = assetInfo;
                _assetLoader = assetLoader;
                _assetStore = assetStore;
            }

            public AssetInfo AssetInfo { get; }
            public Type AssetClassType => _assetLoader.AssetClassType;
            public object? AssetInstance { get; private set; }
            public bool IsLoaded { get; private set; }

            public void Load()
            {
                Debug.Assert(!IsLoaded, "!IsLoaded");

                AssetInstance = _assetLoader.LoadAsset(AssetInfo, _assetStore);
                IsLoaded = true;
            }

            public void Unload()
            {
                Debug.Assert(IsLoaded, "IsLoaded");
                Debug.Assert(AssetInstance != null, nameof(AssetInstance) + " != null");

                _assetLoader.UnloadAsset(AssetInstance);

                AssetInstance = null;
                IsLoaded = false;
            }
        }
    }
}