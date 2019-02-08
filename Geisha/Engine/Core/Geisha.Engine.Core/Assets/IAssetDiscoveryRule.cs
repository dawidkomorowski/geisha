using Geisha.Common;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Core.Assets
{
    public interface IAssetDiscoveryRule
    {
        ISingleOrEmpty<AssetInfo> Discover(IFile file);
    }
}