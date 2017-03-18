using System;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;

namespace Geisha.TestGame.Behaviors
{
    public class FollowEllipse : Behavior
    {
        private double _totalTime;

        public double Velocity { get; set; } = 0.1;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public override void OnUpdate(double deltaTime)
        {
            var transform = Entity.GetComponent<Transform>();
            transform.Translation = new Vector3(Width * Math.Sin(_totalTime * Velocity) + X,
                Height * Math.Cos(_totalTime * Velocity) + Y, transform.Translation.Z);

            _totalTime += deltaTime;
        }
    }
}