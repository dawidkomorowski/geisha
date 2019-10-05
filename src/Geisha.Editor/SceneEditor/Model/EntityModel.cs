using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class EntityModel
    {
        private readonly Entity _entity;
        private readonly List<EntityModel> _children;

        public EntityModel(Entity entity)
        {
            _entity = entity;
            _children = _entity.Children.Select(e => new EntityModel(e)).ToList();
        }

        public string Name => _entity.Name;
        public IReadOnlyCollection<EntityModel> Children => _children.AsReadOnly();
    }
}