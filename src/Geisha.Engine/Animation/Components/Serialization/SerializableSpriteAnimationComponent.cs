using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Animation.Components.Serialization
{
    public sealed class SerializableSpriteAnimationComponent : ISerializableComponent
    {
        public Dictionary<string, Guid>? Animations { get; set; }
        public (string Name, Guid Animation)? CurrentAnimation { get; set; }
        public bool IsPlaying { get; set; }
        public double PlaybackSpeed { get; set; }
        public double Position { get; set; }
    }
}