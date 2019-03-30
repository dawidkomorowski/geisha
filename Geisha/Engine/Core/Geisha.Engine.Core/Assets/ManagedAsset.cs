using System;

namespace Geisha.Engine.Core.Assets
{
    // TODO Add XML documentation.
    public abstract class ManagedAsset<TAsset> : IManagedAsset
        where TAsset : class
    {
        protected ManagedAsset(AssetInfo assetInfo)
        {
            AssetInfo = assetInfo;
        }

        public AssetInfo AssetInfo { get; }
        public object AssetInstance { get; private set; }
        public bool IsLoaded { get; private set; }

        public void Load()
        {
            if (IsLoaded) throw new AssetAlreadyLoadedException(AssetInfo);

            AssetInstance = LoadAsset() ?? throw new LoadAssetReturnedNullException(AssetInfo);
            IsLoaded = true;
        }

        public void Unload()
        {
            if (!IsLoaded) throw new AssetAlreadyUnloadedException(AssetInfo);

            UnloadAsset((TAsset) AssetInstance);

            AssetInstance = null;
            IsLoaded = false;
        }

        protected abstract TAsset LoadAsset();
        protected abstract void UnloadAsset(TAsset asset);
    }

    /// <summary>
    ///     The exception that is thrown when loading an asset that is already loaded by particular managed asset instance.
    /// </summary>
    public sealed class AssetAlreadyLoadedException : Exception
    {
        public AssetAlreadyLoadedException(AssetInfo assetInfo) : base(
            $"Asset for asset info: {assetInfo} was already loaded by this managed asset instance.")
        {
            AssetInfo = assetInfo;
        }

        /// <summary>
        ///     Asset info of asset that loading has failed.
        /// </summary>
        public AssetInfo AssetInfo { get; }
    }

    /// <summary>
    ///     The exception that is thrown when unloading an asset that is already unloaded by particular managed asset instance.
    /// </summary>
    public sealed class AssetAlreadyUnloadedException : Exception
    {
        public AssetAlreadyUnloadedException(AssetInfo assetInfo) : base(
            $"Asset for asset info: {assetInfo} was already unloaded by this managed asset instance.")
        {
            AssetInfo = assetInfo;
        }

        /// <summary>
        ///     Asset info of asset that unloading has failed.
        /// </summary>
        public AssetInfo AssetInfo { get; }
    }

    /// <summary>
    ///     The exception that is thrown when <see cref="ManagedAsset{TAsset}.LoadAsset" /> method returns null while loading
    ///     an asset by particular managed asset instance.
    /// </summary>
    public sealed class LoadAssetReturnedNullException : Exception
    {
        public LoadAssetReturnedNullException(AssetInfo assetInfo) : base(
            $"LoadAsset method returned null for asset info: {assetInfo}. LoadAsset must return not null asset instance.")
        {
            AssetInfo = assetInfo;
        }

        /// <summary>
        ///     Asset info of asset that loading has failed.
        /// </summary>
        public AssetInfo AssetInfo { get; }
    }
}