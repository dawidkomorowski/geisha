using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Sandbox.Behaviors
{
    internal sealed class FollowEllipseComponent : BehaviorComponent
    {
        private double _totalDistance;

        public FollowEllipseComponent(Entity entity) : base(entity)
        {
        }

        public double Velocity { get; set; } = 2;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform2DComponent>();
            transform.Translation = new Vector2(Width * Math.Sin(_totalDistance) + X, Height * Math.Cos(_totalDistance) + Y);

            _totalDistance += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
        }

        protected override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);

            writer.WriteDouble("Velocity", Velocity);
            writer.WriteDouble("X", X);
            writer.WriteDouble("Y", Y);
            writer.WriteDouble("Width", Width);
            writer.WriteDouble("Height", Height);
        }

        protected override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);

            Velocity = reader.ReadDouble("Velocity");
            X = reader.ReadDouble("X");
            Y = reader.ReadDouble("Y");
            Width = reader.ReadDouble("Width");
            Height = reader.ReadDouble("Height");
        }
    }

    internal sealed class FollowEllipseComponentFactory : ComponentFactory<FollowEllipseComponent>
    {
        protected override FollowEllipseComponent CreateComponent(Entity entity) => new FollowEllipseComponent(entity);
    }
}