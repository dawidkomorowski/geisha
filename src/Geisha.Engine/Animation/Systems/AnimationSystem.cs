using System;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

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
                        // TODO compute animation frame
                    }
                }
            }
        }
    }
}