using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Animation.Systems
{
    internal sealed class AnimationSystem : IAnimationSystem, ISceneObserver
    {
        private readonly AnimationState _animationState = new AnimationState();

        #region Implementation of IAnimationSystem

        public void ProcessAnimations(GameTime gameTime)
        {
            _animationState.Update(gameTime);
        }

        #endregion

        #region Implementation of ISceneObserver

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
            switch (component)
            {
                case SpriteAnimationComponent spriteAnimationComponent:
                    _animationState.CreateStateFor(spriteAnimationComponent);
                    break;
                case SpriteRendererComponent spriteRendererComponent:
                    _animationState.CreateStateFor(spriteRendererComponent);
                    break;
            }
        }

        public void OnComponentRemoved(Component component)
        {
            switch (component)
            {
                case SpriteAnimationComponent spriteAnimationComponent:
                    _animationState.RemoveStateFor(spriteAnimationComponent);
                    break;
                case SpriteRendererComponent spriteRendererComponent:
                    _animationState.RemoveStateFor(spriteRendererComponent);
                    break;
            }
        }

        #endregion
    }
}