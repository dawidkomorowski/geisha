using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Components;

namespace Geisha.Engine.Core.SceneModel
{
    public class Entity
    {
        private Entity _parent;
        private readonly List<Entity> _children = new List<Entity>();
        private readonly List<IComponent> _components = new List<IComponent>();

        public Entity Parent
        {
            get { return _parent; }
            set
            {
                _parent?._children.Remove(this);
                _parent = value;
                _parent?._children.Add(this);
            }
        }

        public bool IsRoot => Parent == null;
        public IReadOnlyList<Entity> Children => _children;
        public IReadOnlyList<IComponent> Components => _components;

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            return _components.OfType<TComponent>().Single();
        }

        public IEnumerable<TComponent> GetComponents<TComponent>() where TComponent : IComponent
        {
            return _components.OfType<TComponent>();
        }

        public bool HasComponent<TComponent>() where TComponent : IComponent
        {
            return _components.OfType<TComponent>().Any();
        }

        public void AddComponent(IComponent component)
        {
            _components.Add(component);
        }

        public void RemoveComponent(IComponent component)
        {
            _components.Remove(component);
        }

        public IEnumerable<Entity> GetChildrenRecursively()
        {
            return Children.SelectMany(c => c.GetChildrenRecursively()).Concat(Children);
        }

        public IEnumerable<Entity> GetChildrenRecursivelyIncludingRoot()
        {
            return Children.SelectMany(c => c.GetChildrenRecursivelyIncludingRoot()).Concat(new[] {this});
        }
    }
}