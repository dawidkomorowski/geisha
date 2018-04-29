using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.TestGame.Behaviors
{
    [ComponentDefinition]
    public class Rotate : Behavior
    {
        [PropertyDefinition]
        public double Velocity { get; set; } = Math.PI / 2;

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            transform.Rotation += new Vector3(0, 0, Velocity * Constants.VelocityScale);
        }
    }
}