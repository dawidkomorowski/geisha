using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Animation.Assets
{
    /// <summary>
    ///     Defines <see cref="AssetType" /> values for animation assets used by engine.
    /// </summary>
    public static class AnimationAssetTypes
    {
        /// <summary>
        ///     <see cref="AssetType"/> of sprite animation asset.
        /// </summary>
        public static AssetType SpriteAnimation { get; } = new AssetType(".geisha-sprite-animation");
    }
}