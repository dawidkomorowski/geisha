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

        [PropertyDefinition]
        public double Velocity { get; set; } = 2;

        [PropertyDefinition]
        public double X { get; set; }

        [PropertyDefinition]
        public double Y { get; set; }

        [PropertyDefinition]
        public double Width { get; set; }

        [PropertyDefinition]
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