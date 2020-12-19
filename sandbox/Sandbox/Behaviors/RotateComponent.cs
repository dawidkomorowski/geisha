using System;
using System.Diagnostics;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Sandbox.Behaviors
{
    [SerializableComponent]
    public class RotateComponent : BehaviorComponent
    {
        [SerializableProperty]
        public double Velocity { get; set; } = Math.PI / 2;

        public override void OnFixedUpdate()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<Transform2DComponent>();
            transform.Rotation += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
        }
    }
}