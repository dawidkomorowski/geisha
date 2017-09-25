using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;

namespace Geisha.TestGame.Behaviors
{
    public class FollowEllipse : Behavior
    {
        private double _totalDistance;

        public double Velocity { get; set; } = 2;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            transform.Translation = new Vector3(Width * Math.Sin(_totalDistance) + X,
                Height * Math.Cos(_totalDistance) + Y, transform.Translation.Z);

            _totalDistance += Velocity * Constants.VelocityScale;
        }
    }
}