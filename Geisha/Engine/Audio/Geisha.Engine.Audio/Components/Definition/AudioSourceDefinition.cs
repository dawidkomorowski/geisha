using System;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Components.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="AudioSource" /> that is used in a scene file content.
    /// </summary>
    public sealed class AudioSourceDefinition : IComponentDefinition
    {
        /// <summary>
        ///     Asset id of <see cref="ISound" /> asset.
        /// </summary>
        public Guid SoundAssetId { get; set; }

        /// <summary>
        ///     Defines <see cref="AudioSource.IsPlaying" /> property of <see cref="AudioSource" />.
        /// </summary>
        public bool IsPlaying { get; set; }
    }
}