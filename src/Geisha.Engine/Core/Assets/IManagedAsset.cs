namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Defines interface of object representing an asset that can be managed by <see cref="IAssetStore" />.
    /// </summary>
    /// <remarks>
    ///     Class implementing <see cref="IManagedAsset" /> represents some kind of package for actual asset object
    ///     instance. It is responsible for implementing loading and unloading of actual asset and keeping its instance for
    ///     usage by other clients. Implementation of this interface should be provided for each asset type that should be
    ///     managed and provided by <see cref="IAssetStore" />. To enable <see cref="IAssetStore" /> to manage implementation
    ///     of <see cref="IManagedAsset" /> there must be provided and registered an implementation of corresponding
    ///     <see cref="IManagedAssetFactory" />.
    /// </remarks>
    public interface IManagedAsset
    {
        /// <summary>
        ///     Info of asset managed by this <see cref="IManagedAsset" />.
        /// </summary>
        AssetInfo AssetInfo { get; }

        /// <summary>
        ///     Instance of loaded asset object managed by this <see cref="IManagedAsset" />.
        /// </summary>
        /// <remarks>
        ///     It should be set to not-null object instance after call to <see cref="Load" /> method. It should be set to
        ///     null after call to <see cref="Unload" /> method.
        /// </remarks>
        object? AssetInstance { get; }

        /// <summary>
        ///     Describes managed asset state whether it is loaded or unloaded. If <see cref="IsLoaded" /> is <c>true</c> then
        ///     <see cref="AssetInstance" /> must be not null.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        ///     Loads actual asset object and sets its instance to <see cref="AssetInstance" />.
        /// </summary>
        void Load();

        /// <summary>
        ///     Unloads actual asset object and sets <see cref="AssetInstance" /> to null.
        /// </summary>
        void Unload();
    }
}