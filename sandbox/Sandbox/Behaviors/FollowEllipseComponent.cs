using System;
using System.Diagnostics;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    public class FollowEllipseComponent : BehaviorComponent
    {
        private double _totalDistance;

        public override ComponentId ComponentId { get; } = new ComponentId("Sandbox.Behaviors.FollowEllipseComponent");

        public double Velocity { get; set; } = 2;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public override void OnFixedUpdate()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<Transform2DComponent>();
            transform.Translation = new Vector2(Width * Math.Sin(_totalDistance) + X, Height * Math.Cos(_totalDistance) + Y);

            _totalDistance += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
        }
    }
}