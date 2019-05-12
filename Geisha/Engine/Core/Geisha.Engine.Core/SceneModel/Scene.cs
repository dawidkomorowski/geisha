using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    // TODO Add documentation.
    public class Scene
    {
        private readonly List<Entity> _rootEntities = new List<Entity>();

        public IReadOnlyList<Entity> RootEntities => _rootEntities.AsReadOnly();
        public IEnumerable<Entity> AllEntities => _rootEntities.SelectMany(e => e.GetChildrenRecursivelyIncludingRoot());

        public string ConstructionScript { get; set; }

        public void AddEntity(Entity entity)
        {
            // TODO validate that entity is not already in scene graph or does not allow adding external instances but create them internally?
            entity.Scene = this;
            _rootEntities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Parent = null;
            _rootEntities.Remove(entity);
        }
    }
}