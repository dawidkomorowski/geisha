using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Animation.Systems
{
    internal sealed class AnimationSystem : IAnimationSystem
    {
        public void ProcessAnimations(Scene scene, GameTime gameTime)
        {
            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<SpriteAnimationComponent>())
                {
                    var spriteAnimationComponent = entity.GetComponent<SpriteAnimationComponent>();

                    if (spriteAnimationComponent.AdvanceAnimation(gameTime.DeltaTime))
                    {
                        if (entity.HasComponent<SpriteRendererComponent>())
                        {
                            var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();

                            var sprite = spriteAnimationComponent.ComputeCurrentAnimationFrame();
                            spriteRendererComponent.Sprite = sprite;
                        }
                    }
                }
            }
        }
    }
}