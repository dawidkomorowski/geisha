using Geisha.Common;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Specifies interface of asset discovery rule used in process of automatic assets discovery and registration from
    ///     file system.
    /// </summary>
    /// <remarks>
    ///     Single implementation of this interface per asset file type should be provided. All registered asset discovery
    ///     rules are run for all files and it is recommended that for given file type only one rule actually returns an
    ///     <see cref="AssetInfo" />.
    /// </remarks>
    public interface IAssetDiscoveryRule
    {
        /// <summary>
        ///     Creates single <see cref="AssetInfo" /> for specified <see cref="IFile" /> or returns empty if specified
        ///     <see cref="IFile" /> is not identified as an asset.
        /// </summary>
        /// <param name="file"><see cref="IFile" /> to be identified as an asset.</param>
        /// <returns>Single <see cref="AssetInfo" /> if <paramref name="file" /> is identified as an asset file; otherwise, empty.</returns>
        ISingleOrEmpty<AssetInfo> Discover(IFile file);
    }
}