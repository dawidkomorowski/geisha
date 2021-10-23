using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Input.Assets
{
    /// <summary>
    ///     Defines <see cref="AssetType" /> values for input assets used by engine.
    /// </summary>
    public static class InputAssetTypes
    {
        /// <summary>
        ///     <see cref="AssetType"/> of input mapping asset.
        /// </summary>
        public static AssetType InputMapping { get; } = new AssetType("Geisha.Engine.Input.InputMapping");
    }
}