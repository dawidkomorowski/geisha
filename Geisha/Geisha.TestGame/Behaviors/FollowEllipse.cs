using System;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.TestGame.Behaviors
{
    [SerializableComponent]
    public class FollowEllipse : Behavior
    {
        private double _totalDistance;

        [SerializableProperty]
        public double Velocity { get; set; } = 2;

        [SerializableProperty]
        public double X { get; set; }

        [SerializableProperty]
        public double Y { get; set; }

        [SerializableProperty]
        public double Width { get; set; }

        [SerializableProperty]
        public double Height { get; set; }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            transform.Translation = new Vector3(Width * Math.Sin(_totalDistance) + X,
                Height * Math.Cos(_totalDistance) + Y, transform.Translation.Z);

            _totalDistance += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
        }
    }
}