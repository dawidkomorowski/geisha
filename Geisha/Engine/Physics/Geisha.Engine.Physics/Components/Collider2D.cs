using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     Base class for 2D colliders components.
    /// </summary>
    public abstract class Collider2D : IComponent
    {
        private readonly HashSet<Entity> _collidingEntities = new HashSet<Entity>();

        /// <summary>
        ///     Indicates whether this collider is colliding with some other one.
        /// </summary>
        public bool IsColliding => CollidingEntities.Count > 0;

        /// <summary>
        ///     Collection of all entities colliding with this collider.
        /// </summary>
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