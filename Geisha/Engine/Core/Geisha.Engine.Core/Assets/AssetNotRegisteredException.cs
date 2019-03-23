using System;

namespace Geisha.Engine.Core.Assets
{
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
}