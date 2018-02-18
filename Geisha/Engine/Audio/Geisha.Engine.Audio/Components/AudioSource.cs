using Geisha.Engine.Core.Components;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Components
{
    public class AudioSource : IComponent
    {
        public ISound Sound { get; set; }
        public bool IsPlaying { get; internal set; }
    }
}