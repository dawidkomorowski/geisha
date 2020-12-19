using System;
using System.Diagnostics;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;

namespace Sandbox.Behaviors
{
    [SerializableComponent]
    public sealed class MusicControllerComponent : BehaviorComponent
    {
        private TimeSpan _lastClickTime;
        public IPlayback? Playback { get; set; }

        public override void OnFixedUpdate()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");

            var timeSinceStartUp = GameTime.TimeSinceStartUp;
            if (timeSinceStartUp - _lastClickTime > TimeSpan.FromSeconds(1))
            {
                var input = Entity.GetComponent<InputComponent>();
                if (input.HardwareInput.KeyboardInput.P)
                {
                    Debug.Assert(Playback != null, nameof(Playback) + " != null");

                    if (Playback.IsPlaying)
                    {
                        Playback.Pause();
                    }
                    else
                    {
                        Playback.Play();
                    }

                    _lastClickTime = timeSinceStartUp;
                }
            }
        }
    }
}