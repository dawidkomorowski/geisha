using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Scene is collection of entities that build a single game environment e.g. single level. Scene represents particular
    ///     game state from the engine perspective.
    /// </summary>
    public sealed class Scene
    {
        private readonly List<Entity> _entities = new List<Entity>(); // TODO Would HashSet be faster?
        private readonly List<Entity> _rootEntities = new List<Entity>(); // TODO Would HashSet be faster?

        /// <summary>
        ///     Creates new instance of <see cref="Scene" /> class.
        /// </summary>
        public Scene()
        {
            SceneBehavior = SceneBehavior.CreateEmpty(this);
        }

        /// <summary>
        ///     All entities in the scene that is all root entities and all their children. It can be used to find particular
        ///     entity even if it is only a part of certain complex object.
        /// </summary>
        public IReadOnlyList<Entity> AllEntities => _entities.AsReadOnly();

        /// <summary>
        ///     Root entities of the scene. These typically represent whole logical objects in game world e.g. players, enemies,
        ///     obstacles, projectiles, etc.
        /// </summary>
        public IReadOnlyList<Entity> RootEntities => _rootEntities.AsReadOnly();

        /// <summary>
        ///     Sets or gets <see cref="SceneModel.SceneBehavior" /> used by this <see cref="Scene" />. Default value is empty
        ///     behavior <see cref="SceneModel.SceneBehavior.CreateEmpty" />.
        /// </summary>
        /// <remarks>
        ///     Set <see cref="SceneBehavior" /> to instance of custom <see cref="SceneModel.SceneBehavior" /> implementation
        ///     in order to customize behavior of this <see cref="Scene" /> instance.
        /// </remarks>
        public SceneBehavior SceneBehavior { get; set; }

        /// <summary>
        ///     Creates new root entity in the scene.
        /// </summary>
        /// <returns>New entity created.</returns>
        public Entity CreateEntity()
        {
            var entity = new Entity(this);
            _entities.Add(entity);
            _rootEntities.Add(entity);
            return entity;
        }

        /// <summary>
        ///     Removes specified entity from the scene. If entity is root entity it is removed together with all its children. If
        ///     entity is not root entity it is removed (together with all its children) from children of parent entity.
        /// </summary>
        /// <param name="entity">Entity to be removed from the scene.</param>
        /// <remarks>
        ///     <see cref="Entity" /> removed from the <see cref="SceneModel.Scene" /> should no longer be used. All
        ///     references to such entity should be freed to allow garbage collecting the entity. Entity removed from the scene
        ///     may throw exceptions on usage.
        /// </remarks>
        public void RemoveEntity(Entity entity)
        {
            if (entity.Scene != this)
            {
                throw new ArgumentException("Cannot remove entity created by another scene.");
            }

            if (entity.IsRemoved) return;

            while (entity.Children.Count != 0)
            {
                RemoveEntity(entity.Children[0]);
            }

            entity.Parent = null;
            _entities.Remove(entity);
            _rootEntities.Remove(entity);
            entity.IsRemoved = true;
        }

        internal void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
            if (newParent is null)
            {
                _rootEntities.Add(entity);
            }

            if (newParent != null && oldParent == null)
            {
                _rootEntities.Remove(entity);
            }
        }

        internal void OnLoaded()
        {
            SceneBehavior.OnLoaded();
        }
    }
}