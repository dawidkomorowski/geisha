using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components
{
    public abstract class Collider2D : IComponent
    {
        private readonly HashSet<Entity> _collidingEntities = new HashSet<Entity>();

        public bool IsColliding => CollidingEntities.Count > 0;
        public IReadOnlyCollection<Entity> CollidingEntities => _collidingEntities;

        internal void ClearCollidingEntities()
        {
            _collidingEntities.Clear();
        }

        internal void AddCollidingEntity(Entity entity)
        {
            _collidingEntities.Add(entity);
        }
    }
}