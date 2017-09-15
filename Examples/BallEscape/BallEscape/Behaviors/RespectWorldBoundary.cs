using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Rendering.Components;

namespace BallEscape.Behaviors
{
    public class RespectWorldBoundary : Behavior
    {
        private const int WorldWidth = 1280;
        private const int WorldHeight = 720;
        private const int WorldBorder = 5;

        private const int HalfWorldWidth = WorldWidth / 2;
        private const int HalfWorldHeight = WorldHeight / 2;

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            var spriteRenderer = Entity.GetComponent<SpriteRenderer>();

            var radius = spriteRenderer.Sprite.SourceTexture.Dimension.X / 2;

            if (transform.Translation.X + radius > HalfWorldWidth - WorldBorder)
                transform.Translation = new Vector3(HalfWorldWidth - WorldBorder - radius, transform.Translation.Y, transform.Translation.Z);
            if (transform.Translation.X - radius < -HalfWorldWidth + WorldBorder)
                transform.Translation = new Vector3(-HalfWorldWidth + WorldBorder + radius, transform.Translation.Y, transform.Translation.Z);
            if (transform.Translation.Y + radius > HalfWorldHeight - WorldBorder)
                transform.Translation = new Vector3(transform.Translation.X, HalfWorldHeight - WorldBorder - radius, transform.Translation.Z);
            if (transform.Translation.Y - radius < -HalfWorldHeight + WorldBorder)
                transform.Translation = new Vector3(transform.Translation.X, -HalfWorldHeight + WorldBorder + radius, transform.Translation.Z);
        }
    }
}