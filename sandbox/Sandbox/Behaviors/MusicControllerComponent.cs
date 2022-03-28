using System;
using System.Diagnostics;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;

namespace Sandbox.Behaviors
{
    internal sealed class MusicControllerComponent : BehaviorComponent
    {
        private TimeSpan _lastClickTime;

        public MusicControllerComponent(Entity entity) : base(entity)
        {
        }

        public IPlayback? Playback { get; set; }

        public override void OnFixedUpdate()
        {
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

    internal sealed class MusicControllerComponentFactory : ComponentFactory<MusicControllerComponent>
    {
        protected override MusicControllerComponent CreateComponent(Entity entity) => new MusicControllerComponent(entity);
    }
}