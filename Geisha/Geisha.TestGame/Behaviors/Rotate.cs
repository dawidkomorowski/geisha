using System;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;

namespace Geisha.TestGame.Behaviors
{
    public class Rotate : Behavior
    {
        public double Velocity { get; set; } = Math.PI / 110;

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            transform.Rotation += new Vector3(0, 0, Velocity);
        }
    }
}