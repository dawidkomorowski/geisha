using System;
using System.Diagnostics;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Sandbox.Behaviors
{
    internal sealed class FollowEllipseComponent : BehaviorComponent
    {
        private double _totalDistance;

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

        protected override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);

            componentDataWriter.WriteDouble("Velocity", Velocity);
            componentDataWriter.WriteDouble("X", X);
            componentDataWriter.WriteDouble("Y", Y);
            componentDataWriter.WriteDouble("Width", Width);
            componentDataWriter.WriteDouble("Height", Height);
        }

        protected override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);

            Velocity = componentDataReader.ReadDouble("Velocity");
            X = componentDataReader.ReadDouble("X");
            Y = componentDataReader.ReadDouble("Y");
            Width = componentDataReader.ReadDouble("Width");
            Height = componentDataReader.ReadDouble("Height");
        }
    }

    internal sealed class FollowEllipseComponentFactory : ComponentFactory<FollowEllipseComponent>
    {
        protected override FollowEllipseComponent CreateComponent() => new FollowEllipseComponent();
    }
}