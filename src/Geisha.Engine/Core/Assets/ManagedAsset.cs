using System;
using System.Diagnostics;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Abstract base class simplifying implementation of <see cref="IManagedAsset" /> interface. It provides most of the
    ///     contract required by <see cref="IManagedAsset" /> and requires only implementation of <see cref="LoadAsset" /> and
    ///     <see cref="UnloadAsset" />.
    /// </summary>
    /// <typeparam name="TAsset">Asset type the managed asset object will handle.</typeparam>
    public abstract class ManagedAsset<TAsset> : IManagedAsset
        where TAsset : class
    {
        /// <summary>
        ///     Initializes <see cref="AssetInfo" /> property with specified <paramref name="assetInfo" /> parameter.
        /// </summary>
        /// <param name="assetInfo">Info of asset that will be managed by this <see cref="ManagedAsset{TAsset}" /> instance.</param>
        protected ManagedAsset(AssetInfo assetInfo)
        {
            AssetInfo = assetInfo;
        }

        /// <summary>
        ///     Info of asset managed by this <see cref="ManagedAsset{TAsset}" />.
        /// </summary>
        public AssetInfo AssetInfo { get; }

        /// <summary>
        ///     Instance of loaded asset object managed by this <see cref="ManagedAsset{TAsset}" />.
        /// </summary>
        /// ///
        /// <remarks>
        ///     It is set to not-null object instance after call to <see cref="Load" /> method. It is set to null after call to
        ///     <see cref="Unload" /> method.
        /// </remarks>
        public object? AssetInstance { get; private set; }

        /// <summary>
        ///     Describes managed asset state whether it is loaded or unloaded. If <see cref="IsLoaded" /> is <c>true</c> then
        ///     <see cref="AssetInstance" /> is not null.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        ///     Loads actual asset object and sets its instance to <see cref="AssetInstance" />.
        /// </summary>
        public void Load()
        {
            if (IsLoaded) throw new AssetAlreadyLoadedException(AssetInfo);

            AssetInstance = LoadAsset() ?? throw new LoadAssetReturnedNullException(AssetInfo);
            IsLoaded = true;
        }

        /// <summary>
        ///     Unloads actual asset object and sets <see cref="AssetInstance" /> to null.
        /// </summary>
        public void Unload()
        {
            if (!IsLoaded) throw new AssetAlreadyUnloadedException(AssetInfo);

            Debug.Assert(AssetInstance != null, nameof(AssetInstance) + " != null");
            UnloadAsset((TAsset) AssetInstance);

            AssetInstance = null;
            IsLoaded = false;
        }

        /// <summary>
        ///     Implementation of this method should load an asset object instance based on <see cref="AssetInfo" /> and return it.
        /// </summary>
        /// <returns>Loaded asset object instance.</returns>
        protected abstract TAsset LoadAsset();

        /// <summary>
        ///     Implementation of this method should unload (perform some kind of disposal logic) asset object instance provided as
        ///     a parameter.
        /// </summary>
        /// <param name="asset">Loaded asset object instance to be unloaded.</param>
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