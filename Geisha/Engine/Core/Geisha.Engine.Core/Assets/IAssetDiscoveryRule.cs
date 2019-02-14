using Geisha.Common;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Assets
{
    // TODO Add xml documentation.
    public interface IAssetDiscoveryRule
    {
        ISingleOrEmpty<AssetInfo> Discover(IFile file);
    }
}