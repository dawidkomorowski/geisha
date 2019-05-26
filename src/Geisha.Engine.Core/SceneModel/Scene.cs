using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Scene is collection of entities that build a single game environment e.g. single level. Scene represents particular
    ///     game state from the engine perspective.
    /// </summary>
    public sealed class Scene
    {
        private readonly List<Entity> _rootEntities = new List<Entity>();

        /// <summary>
        ///     Root entities of the scene. These typically represent whole logical objects in game world e.g. players, enemies,
        ///     obstacles, projectiles, etc.
        /// </summary>
        public IReadOnlyList<Entity> RootEntities => _rootEntities.AsReadOnly();

        /// <summary>
        ///     All entities in the scene that is all root entities and all their children. It can be used to find particular
        ///     entity even if it is only a part of certain complex object.
        /// </summary>
        public IEnumerable<Entity> AllEntities => _rootEntities.SelectMany(e => e.GetChildrenRecursivelyIncludingRoot());

        /// <summary>
        ///     Name of construction script set for the scene. If not null, after scene is loaded, this construction script will be
        ///     executed for this scene before scene is processed by systems.
        /// </summary>
        public string ConstructionScript { get; set; }

        /// <summary>
        ///     Adds specified entity as a root entity to the scene.
        /// </summary>
        /// <param name="entity">Entity to be added to scene as root entity.</param>
        public void AddEntity(Entity entity)
        {
            // TODO validate that entity is not already in scene graph or does not allow adding external instances but create them internally?
            entity.Scene = this;
            _rootEntities.Add(entity);
        }

        /// <summary>
        ///     Removes specified entity from the scene. If entity is root entity it is removed together with all its children. If
        ///     entity is not root entity it is removed from children of parent entity.
        /// </summary>
        /// <param name="entity">Entity to be removed from the scene.</param>
        public void RemoveEntity(Entity entity)
        {
            entity.Parent = null;
            _rootEntities.Remove(entity);
        }
    }
}