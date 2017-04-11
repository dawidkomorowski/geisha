using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    public class Scene
    {
        private readonly List<Entity> _rootEntities = new List<Entity>();
        public IReadOnlyList<Entity> RootEntities => _rootEntities.AsReadOnly();
        public IEnumerable<Entity> AllEntities => _rootEntities.SelectMany(e => e.GetChildrenRecursivelyIncludingRoot());

        public void AddEntity(Entity entity)
        {
            entity.Scene = this;
            _rootEntities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Scene = null;
            _rootEntities.Remove(entity);
        }
    }
}