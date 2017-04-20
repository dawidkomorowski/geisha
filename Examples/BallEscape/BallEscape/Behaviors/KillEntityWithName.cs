using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Rendering.Components;

namespace BallEscape.Behaviors
{
    public class KillEntityWithName : Behavior
    {
        private readonly string[] _names;

        public KillEntityWithName(params string[] names)
        {
            _names = names;
        }

        public override void OnFixedUpdate()
        {
            if(Entity.Scene == null) return; // Workaround for bug #49. If enemy dies it is removed from scene thus Entity.Scene is null.
            // Unfortunately if entity is removed from scene it is still handled in update.

            var transform = Entity.GetComponent<Transform>();
            var spriteRenderer = Entity.GetComponent<SpriteRenderer>();

            var radius = spriteRenderer.Sprite.SourceTexture.Dimension.X / 2;

            foreach (var entity in Entity.Scene.RootEntities.ToList())
            {
                if (_names.Contains(entity.Name))
                {
                    var entityTransform = entity.GetComponent<Transform>();
                    var entitySpriteRenderer = entity.GetComponent<SpriteRenderer>();

                    var entityRadius = entitySpriteRenderer.Sprite.SourceTexture.Dimension.X / 2;
                    var distance = entityTransform.Translation.Distance(transform.Translation);

                    if (distance < radius + entityRadius)
                    {
                        Entity.Scene.RemoveEntity(entity);
                    }
                }
            }
        }
    }
}