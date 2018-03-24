﻿using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;

namespace Geisha.Engine.Core.Assets
{
    public interface IAssetLoader
    {
        object Load(string filePath);
    }

    public interface IAssetLoaderProvider
    {
        IAssetLoader GetLoaderFor(Type assetType);
    }

    public interface IAssetStore
    {
        TAsset GetAsset<TAsset>(Guid assetId);
        Guid GetAssetId(object asset);
        void RegisterAsset(AssetInfo assetInfo);
    }

    internal class AssetStore : IAssetStore
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AssetStore));
        private readonly Dictionary<object, Guid> _assetIds = new Dictionary<object, Guid>();
        private readonly IAssetLoaderProvider _assetLoaderProvider;
        private readonly Dictionary<AssetInfo, object> _loadedAssets = new Dictionary<AssetInfo, object>();
        private readonly HashSet<AssetInfo> _registeredAssets = new HashSet<AssetInfo>();

        public AssetStore(IAssetLoaderProvider assetLoaderProvider)
        {
            _assetLoaderProvider = assetLoaderProvider;
        }

        public TAsset GetAsset<TAsset>(Guid assetId)
        {
            var assetInfo = _registeredAssets.SingleOrDefault(ai => ai.AssetType == typeof(TAsset) && ai.AssetId == assetId);
            if (assetInfo == null) throw new GeishaEngineException($"Asset not found for type {typeof(TAsset).FullName} and id {assetId}.");

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

        public Guid GetAssetId(object asset)
        {
            if (!_assetIds.TryGetValue(asset, out var assetId))
                throw new ArgumentException($"Given asset was not loaded by this asset store.", nameof(asset));

            return assetId;
        }

        public void RegisterAsset(AssetInfo assetInfo)
        {
            if (_registeredAssets.Add(assetInfo)) return;

            Log.Warn(
                $"Asset already registered, wil be overridden. Existing asset info: {_registeredAssets.Single(ai => ai == assetInfo)}. New asset info: {assetInfo}");

            _registeredAssets.Remove(assetInfo);
            _registeredAssets.Add(assetInfo);
        }
    }
}