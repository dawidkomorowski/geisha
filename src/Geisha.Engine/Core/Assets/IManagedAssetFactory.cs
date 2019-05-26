using Geisha.Common;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Defines interface of factory producing <see cref="IManagedAsset" /> objects. Factory implementation will serve
    ///     <see cref="IAssetStore" /> to create <see cref="IManagedAsset" /> objects.
    /// </summary>
    /// <remarks>
    ///     When implementing <see cref="IManagedAsset" /> interface there should be provided and registered an
    ///     implementation of <see cref="IManagedAssetFactory" /> interface. It is required for <see cref="IAssetStore" /> to
    ///     manage assets correctly.
    ///     <br /><br />
    ///     For given <see cref="AssetInfo" /> there should be only single factory implementation
    ///     registered that returns non empty result, i.e. for certain asset type there is only one factory that produces
    ///     assets while for that type other factories return empty.
    /// </remarks>
    public interface IManagedAssetFactory
    {
        /// <summary>
        ///     Creates and returns single instance of <see cref="IManagedAsset" /> for given <paramref name="assetInfo" /> or
        ///     returns empty result if <paramref name="assetInfo" /> does not match the factory.
        /// </summary>
        /// <param name="assetInfo">Asset info for which to create a managed asset.</param>
        /// <param name="assetStore">
        ///     Asset store instance provided for creation of composite assets that require other existing
        ///     assets.
        /// </param>
        /// <returns>
        ///     Single instance of <see cref="IManagedAsset" /> if <paramref name="assetInfo" /> matches the factory;
        ///     otherwise empty result.
        /// </returns>
        ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore);
    }
}