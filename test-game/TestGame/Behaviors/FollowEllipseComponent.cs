using System;
using System.Diagnostics;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace TestGame.Behaviors
{
    [SerializableComponent]
    public class FollowEllipseComponent : BehaviorComponent
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
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<TransformComponent>();
            transform.Translation = new Vector3(Width * Math.Sin(_totalDistance) + X,
                Height * Math.Cos(_totalDistance) + Y, transform.Translation.Z);

            _totalDistance += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
        }
    }
}