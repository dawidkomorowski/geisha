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

        /// <summary>
        ///     <see cref="AssetType"/> of sprite asset.
        /// </summary>
        public static AssetType Sprite { get; } = new AssetType("Geisha.Engine.Rendering.Sprite");
    }
}