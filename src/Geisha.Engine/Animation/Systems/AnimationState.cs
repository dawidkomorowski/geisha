using System.Collections.Generic;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Animation.Systems
{
    internal sealed class AnimationState
    {
        private readonly List<AnimationNode> _nodes = new List<AnimationNode>();
        private readonly Dictionary<Entity, AnimationNode> _index = new Dictionary<Entity, AnimationNode>();
        private readonly Dictionary<Entity, SpriteRendererComponent> _pendingSpriteRendererComponents = new Dictionary<Entity, SpriteRendererComponent>();

        public void CreateStateFor(SpriteAnimationComponent spriteAnimationComponent)
        {
            var node = new AnimationNode(spriteAnimationComponent);
            _nodes.Add(node);
            _index.Add(node.Entity, node);

            if (_pendingSpriteRendererComponents.TryGetValue(node.Entity, out var spriteRendererComponent))
            {
                node.SpriteRendererComponent = spriteRendererComponent;
                _pendingSpriteRendererComponents.Remove(node.Entity);
            }
        }

        public void CreateStateFor(SpriteRendererComponent spriteRendererComponent)
        {
            if (_index.TryGetValue(spriteRendererComponent.Entity, out var node))
            {
                node.SpriteRendererComponent = spriteRendererComponent;
            }
            else
            {
                _pendingSpriteRendererComponents.Add(spriteRendererComponent.Entity, spriteRendererComponent);
            }
        }

        public void RemoveStateFor(SpriteAnimationComponent spriteAnimationComponent)
        {
            var node = _index[spriteAnimationComponent.Entity];
            _nodes.Remove(node);
            _index.Remove(node.Entity);
        }

        public void RemoveStateFor(SpriteRendererComponent spriteRendererComponent)
        {
            if (_index.TryGetValue(spriteRendererComponent.Entity, out var node))
            {
                node.SpriteRendererComponent = null;
            }
            else
            {
                _pendingSpriteRendererComponents.Remove(spriteRendererComponent.Entity);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var node in _nodes)
            {
                node.Update(gameTime);
            }
        }

        private sealed class AnimationNode
        {
            public AnimationNode(SpriteAnimationComponent spriteAnimationComponent)
            {
                Entity = spriteAnimationComponent.Entity;
                SpriteAnimationComponent = spriteAnimationComponent;
            }

            public Entity Entity { get; }
            private SpriteAnimationComponent SpriteAnimationComponent { get; }
            public SpriteRendererComponent? SpriteRendererComponent { get; set; }

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