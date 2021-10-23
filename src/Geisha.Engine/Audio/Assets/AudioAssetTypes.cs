using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Audio.Assets
{
    /// <summary>
    ///     Defines <see cref="AssetType" /> values for audio assets used by engine.
    /// </summary>
    public static class AudioAssetTypes
    {
        /// <summary>
        ///     <see cref="AssetType"/> of sound asset.
        /// </summary>
        public static AssetType Sound { get; } = new AssetType("Geisha.Engine.Audio.Sound");
    }
}