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
        ///     Creates new instance of <see cref="Scene" /> class.
        /// </summary>
        public Scene()
        {
            SceneBehavior = SceneBehavior.CreateEmpty(this);
        }

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
        ///     Sets or gets <see cref="SceneModel.SceneBehavior" /> used by this <see cref="Scene" />. Default value is empty
        ///     behavior <see cref="SceneModel.SceneBehavior.CreateEmpty" />.
        /// </summary>
        /// <remarks>
        ///     Set <see cref="SceneBehavior" /> to instance of custom <see cref="SceneModel.SceneBehavior" /> implementation
        ///     in order to customize behavior of this <see cref="Scene" /> instance.
        /// </remarks>
        public SceneBehavior SceneBehavior { get; set; }

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

        internal void OnLoaded()
        {
            SceneBehavior.OnLoaded();
        }
    }
}