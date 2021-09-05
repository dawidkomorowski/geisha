using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Rendering.Assets
{
    /// <summary>
    ///     Defines <see cref="AssetType" /> values for rendering assets used by engine.
    /// </summary>
    public static class RenderingAssetTypes
    {
        /// <summary>
        ///     <see cref="AssetType"/> of texture asset.
        /// </summary>
        public static AssetType Texture { get; } = new AssetType("Geisha.Engine.Rendering.Texture");
    }
}