using System;
using System.Diagnostics;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace TestGame.Behaviors
{
    [SerializableComponent]
    public sealed class StopAnimationAfterSecondsComponent : BehaviorComponent
    {
        private TimeSpan _timeSinceStarted;

        public override void OnStart()
        {
            _timeSinceStarted = TimeSpan.Zero;
        }

        public override void OnUpdate(GameTime gameTime)
        {
            _timeSinceStarted += gameTime.DeltaTime;

            if (_timeSinceStarted.TotalSeconds > 5)
            {
                Debug.Assert(Entity != null, nameof(Entity) + " != null");
                var animationComponent = Entity.GetComponent<SpriteAnimationComponent>();
                animationComponent.Stop();
            }
        }
    }
}