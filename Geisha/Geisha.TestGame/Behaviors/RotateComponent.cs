using System;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.TestGame.Behaviors
{
    [SerializableComponent]
    public class RotateComponent : BehaviorComponent
    {
        [SerializableProperty]
        public double Velocity { get; set; } = Math.PI / 2;

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.Rotation += new Vector3(0, 0, Velocity * GameTime.FixedDeltaTime.TotalSeconds);
        }
    }
}