using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Animation.Components.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="SpriteAnimationComponent" /> that is used in a scene file content.
    /// </summary>
    public sealed class SerializableSpriteAnimationComponent : ISerializableComponent
    {
        /// <summary>
        ///     Represents <see cref="SpriteAnimationComponent.Animations" /> property of <see cref="SpriteAnimationComponent" />.
        /// </summary>
        public Dictionary<string, Guid>? Animations { get; set; }

        /// <summary>
        ///     Represents <see cref="SpriteAnimationComponent.CurrentAnimation" /> property of
        ///     <see cref="SpriteAnimationComponent" />.
        /// </summary>
        public (string Name, Guid Animation)? CurrentAnimation { get; set; }

        /// <summary>
        ///     Represents <see cref="SpriteAnimationComponent.IsPlaying" /> property of <see cref="SpriteAnimationComponent" />.
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        ///     Represents <see cref="SpriteAnimationComponent.PlaybackSpeed" /> property of
        ///     <see cref="SpriteAnimationComponent" />.
        /// </summary>
        public double PlaybackSpeed { get; set; }

        /// <summary>
        ///     Represents <see cref="SpriteAnimationComponent.Position" /> property of <see cref="SpriteAnimationComponent" />.
        /// </summary>
        public double Position { get; set; }

        /// <summary>
        ///     Represents <see cref="SpriteAnimationComponent.PlayInLoop" /> property of <see cref="SpriteAnimationComponent" />.
        /// </summary>
        public bool PlayInLoop { get; set; }
    }
}