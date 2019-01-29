using System;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    public class Scene
    {
        private readonly List<Entity> _rootEntities = new List<Entity>();
        private ISceneConstructionScript _constructionScript = new EmptySceneConstructionScript();

        public IReadOnlyList<Entity> RootEntities => _rootEntities.AsReadOnly();
        public IEnumerable<Entity> AllEntities => _rootEntities.SelectMany(e => e.GetChildrenRecursivelyIncludingRoot());

        public ISceneConstructionScript ConstructionScript
        {
            get => _constructionScript;
            set => _constructionScript = value ?? throw new ArgumentNullException();
        }

        public void AddEntity(Entity entity)
        {
            // TODO validate that entity is not already in scene graph
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