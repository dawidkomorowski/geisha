﻿using System;

namespace Geisha.Engine.Animation.Assets.Serialization
{
    /// <summary>
    ///     Defines sprite animation content to be used to load <see cref="SpriteAnimation" /> from a file into memory.
    /// </summary>
    public sealed class SpriteAnimationAssetContent
    {
        /// <summary>
        ///     Frames data for <see cref="SpriteAnimation" />.
        /// </summary>
        public Frame[]? Frames { get; set; }

        /// <summary>
        ///     Duration data for <see cref="SpriteAnimation" /> defined in ticks time units of <see cref="TimeSpan" />.
        /// </summary>
        public long DurationTicks { get; set; }

        /// <summary>
        ///     Represents sprite animation frame element in sprite animation file content.
        /// </summary>
        public sealed class Frame
        {
            /// <summary>
            ///     Asset id of sprite asset.
            /// </summary>
            public Guid SpriteAssetId { get; set; }

            /// <summary>
            ///     Duration data for <see cref="SpriteAnimationFrame" />.
            /// </summary>
            public double Duration { get; set; }
        }
    }
}