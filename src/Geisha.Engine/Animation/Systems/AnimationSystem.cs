using System.Collections.Generic;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Animation.Systems
{
    internal sealed class AnimationSystem : IAnimationSystem, ISceneObserver
    {
        private readonly List<AnimationNode> _animationNodes = new List<AnimationNode>();

        #region Implementation of IAnimationSystem

        public void ProcessAnimations(GameTime gameTime)
        {
            // foreach creates copy of the node
            foreach (var animationNode in _animationNodes)
            {
                animationNode.Update(gameTime);
            }
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
            if (component is SpriteAnimationComponent spriteAnimationComponent)
            {
                if (component.Entity.HasComponent<SpriteRendererComponent>())
                {
                    var spriteRendererComponent = component.Entity.GetComponent<SpriteRendererComponent>();
                    var animationNode = new AnimationNode(spriteAnimationComponent, spriteRendererComponent);
                    _animationNodes.Add(animationNode);
                }
                else
                {
                    var animationNode = new AnimationNode(spriteAnimationComponent);
                    _animationNodes.Add(animationNode);
                }
            }
            else if (component is SpriteRendererComponent spriteRendererComponent && component.Entity.HasComponent<SpriteAnimationComponent>())
            {
                var existingSpriteAnimationComponent = component.Entity.GetComponent<SpriteAnimationComponent>();
                _animationNodes.RemoveAll(an => an.SpriteAnimationComponent == existingSpriteAnimationComponent);

                var animationNode = new AnimationNode(existingSpriteAnimationComponent, spriteRendererComponent);
                _animationNodes.Add(animationNode);
            }
        }

        public void OnComponentRemoved(Component component)
        {
            if (component is SpriteAnimationComponent spriteAnimationComponent)
            {
                _animationNodes.RemoveAll(an => an.SpriteAnimationComponent == spriteAnimationComponent);
            }
            else if (component is SpriteRendererComponent && component.Entity.HasComponent<SpriteAnimationComponent>())
            {
                var existingSpriteAnimationComponent = component.Entity.GetComponent<SpriteAnimationComponent>();
                _animationNodes.RemoveAll(an => an.SpriteAnimationComponent == existingSpriteAnimationComponent);

                var animationNode = new AnimationNode(existingSpriteAnimationComponent);
                _animationNodes.Add(animationNode);
            }
        }

        #endregion

        // foreach creates copy of the node
        private readonly struct AnimationNode
        {
            public AnimationNode(SpriteAnimationComponent spriteAnimationComponent)
            {
                SpriteAnimationComponent = spriteAnimationComponent;
                SpriteRendererComponent = null;
            }

            public AnimationNode(SpriteAnimationComponent spriteAnimationComponent, SpriteRendererComponent spriteRendererComponent)
            {
                SpriteAnimationComponent = spriteAnimationComponent;
                SpriteRendererComponent = spriteRendererComponent;
            }

            public SpriteAnimationComponent SpriteAnimationComponent { get; }
            private SpriteRendererComponent? SpriteRendererComponent { get; }

            public void Update(GameTime gameTime)
            {
                SpriteAnimationComponent.AdvanceAnimation(gameTime.DeltaTime);

                if (SpriteRendererComponent != null)
                {
                    var sprite = SpriteAnimationComponent.ComputeCurrentAnimationFrame();
                    SpriteRendererComponent.Sprite = sprite;
                }
            }
        }
    }
}