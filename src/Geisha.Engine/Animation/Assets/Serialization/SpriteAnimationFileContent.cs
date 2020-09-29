using System;

namespace Geisha.Engine.Animation.Assets.Serialization
{
    public sealed class SpriteAnimationFileContent
    {
        public Guid AssetId { get; set; }
        public Frame[]? Frames { get; set; }
        public long DurationTicks { get; set; }

        public sealed class Frame
        {
            public Guid SpriteAssetId { get; set; }
            public double Duration { get; set; }
        }
    }
}