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

                    if (spriteAnimationComponent.CurrentAnimation.HasValue && spriteAnimationComponent.IsPlaying)
                    {
                        var currentAnimation = spriteAnimationComponent.CurrentAnimation.Value.Animation;
                        var positionDelta = gameTime.DeltaTime / currentAnimation.Duration;
                        var finalPosition = spriteAnimationComponent.Position + positionDelta * spriteAnimationComponent.PlaybackSpeed;

                        if (finalPosition >= 1.0)
                        {
                            finalPosition = 1.0;
                            spriteAnimationComponent.Pause();

                            var currentAnimationName = spriteAnimationComponent.CurrentAnimation.Value.Name;
                            spriteAnimationComponent.OnAnimationCompleted(new SpriteAnimationCompletedEventArgs(currentAnimationName, currentAnimation));
                        }

                        spriteAnimationComponent.Position = finalPosition;
                    }
                }
            }
        }
    }
}