using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Entity represents any object in the game scene. It's behavior and functionality is defined by attached components
    ///     which are processed by systems.
    /// </summary>
    public sealed class Entity
    {
        private readonly List<Entity> _children = new();
        private readonly IComponentFactoryProvider _componentFactoryProvider;
        private readonly List<Component> _components = new();
        private readonly Dictionary<Type, List<Component>> _componentsByType = new();
        private Entity? _parent;

        /// <summary>
        ///     Internal API for <see cref="SceneModel.Scene" /> class.
        /// </summary>
        internal Entity(Scene scene, IComponentFactoryProvider componentFactoryProvider)
        {
            Scene = scene;
            _componentFactoryProvider = componentFactoryProvider;
        }

        /// <summary>
        ///     Scene that this entity is part of.
        /// </summary>
        public Scene Scene { get; }

        /// <summary>
        ///     Returns <c>true</c> if entity was removed from the <see cref="Scene" />; otherwise returns <c>false</c>.
        /// </summary>
        /// <remarks>
        ///     <see cref="Entity" /> removed from the <see cref="SceneModel.Scene" /> should no longer be used. All
        ///     references to such entity should be freed to allow garbage collecting the entity. Entity removed from the scene
        ///     may throw exceptions on usage.
        /// </remarks>
        public bool IsRemoved { get; internal set; }

        /// <summary>
        ///     Name of entity. Can be used to uniquely identify entities by <c>string</c> names or include some debugging
        ///     information.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     Entity that is parent of this entity (this entity is child of parent entity). Returns <c>null</c> if entity is root
        ///     of entity tree.
        /// </summary>
        public Entity? Parent
        {
            get => _parent;
            set
            {
                ThrowIfEntityIsRemovedFromTheScene();

                if (value == _parent) return;

                if (value == this)
                {
                    throw new ArgumentException("Cannot set entity as Parent of itself.");
                }

                if (value != null && value.Scene != Scene)
                {
                    throw new ArgumentException("Cannot set Parent to entity created by another scene.");
                }

                var oldParent = _parent;

                _parent?._children.Remove(this);
                _parent = value;
                _parent?._children.Add(this);

                Scene.OnEntityParentChanged(this, oldParent, _parent);
            }
        }

        /// <summary>
        ///     Defines whether this entity is root of entity tree. True if entity is root of entity tree; false otherwise.
        /// </summary>
        [MemberNotNullWhen(false, nameof(Parent))]
        public bool IsRoot => Parent == null;

        /// <summary>
        ///     Root of entity tree that this entity is part of. Returns this entity if it is root of entity tree.
        /// </summary>
        public Entity Root => IsRoot ? this : Parent.Root;

        /// <summary>
        ///     Entities that are children of this entity.
        /// </summary>
        public IReadOnlyList<Entity> Children => _children; // TODO AsReadOnly allocates wrapper and it may hurt performance.

        public IReadOnlyList<Entity> ChildrenBaseline => _children.AsReadOnly();

        /// <summary>
        ///     Components attached to this entity.
        /// </summary>
        public IReadOnlyList<Component> Components => _components; // TODO AsReadOnly allocates wrapper and it may hurt performance.

        public IReadOnlyList<Component> ComponentsBaseline => _components.AsReadOnly();

        /// <summary>
        ///     Creates new entity as a child of this entity.
        /// </summary>
        /// <returns>New entity created.</returns>
        /// <remarks>Creates new entity in the <see cref="Scene" /> and sets its <see cref="Parent" /> to this entity.</remarks>
        public Entity CreateChildEntity()
        {
            ThrowIfEntityIsRemovedFromTheScene();

            var entity = Scene.CreateEntity();
            entity.Parent = this;
            return entity;
        }

        /// <summary>
        ///     Returns all children of entity including children of children effectively iterating through whole subtree.
        /// </summary>
        /// <returns>Entities that are all children of this entity including children of children.</returns>
        public IEnumerable<Entity> GetChildrenRecursively()
        {
            return Children.SelectMany(c => c.GetChildrenRecursively()).Concat(Children);
        }

        /// <summary>
        ///     Returns this entity and all its children including children of children effectively iterating through whole
        ///     subtree.
        /// </summary>
        /// <returns>Entities collection that contains this entity and all its children including children of children.</returns>
        public IEnumerable<Entity> GetChildrenRecursivelyIncludingRoot()
        {
            return Children.SelectMany(c => c.GetChildrenRecursivelyIncludingRoot()).Concat(new[] { this });
        }

        /// <summary>
        ///     Creates new instance of specified component and attaches it to entity.
        /// </summary>
        /// <typeparam name="TComponent">Type of component to create.</typeparam>
        /// <returns>New instance of component.</returns>
        public TComponent CreateComponent<TComponent>() where TComponent : Component
        {
            ThrowIfEntityIsRemovedFromTheScene();

            var component = _componentFactoryProvider.Get<TComponent>().Create(this);
            _components.Add(component);

            if (!_componentsByType.TryGetValue(typeof(TComponent), out var componentsOfType))
            {
                componentsOfType = new List<Component>(1);
                _componentsByType.Add(typeof(TComponent), componentsOfType);
            }

            componentsOfType.Add(component);

            Scene.OnComponentCreated(component);

            return (TComponent)component;
        }

        /// <summary>
        ///     Creates new instance of specified component and attaches it to entity.
        /// </summary>
        /// <param name="componentId"><see cref="ComponentId" /> of component to create.</param>
        /// <returns>New instance of component.</returns>
        public Component CreateComponent(ComponentId componentId)
        {
            ThrowIfEntityIsRemovedFromTheScene();

            var component = _componentFactoryProvider.Get(componentId).Create(this);
            _components.Add(component);

            if (!_componentsByType.TryGetValue(component.GetType(), out var componentsOfType))
            {
                componentsOfType = new List<Component>(1);
                _componentsByType.Add(component.GetType(), componentsOfType);
            }

            componentsOfType.Add(component);

            Scene.OnComponentCreated(component);

            return component;
        }

        /// <summary>
        ///     Removes specified component instance from entity.
        /// </summary>
        /// <param name="component">Component instance to be removed from entity.</param>
        public void RemoveComponent(Component component)
        {
            ThrowIfEntityIsRemovedFromTheScene();

            _components.Remove(component);

            if (_componentsByType.TryGetValue(component.GetType(), out var componentsOfType))
            {
                componentsOfType.Remove(component);
            }

            Scene.OnComponentRemoved(component);
        }

        /// <summary>
        ///     Returns component of specified type. Throws exception if there are multiple or none of requested components.
        /// </summary>
        /// <typeparam name="TComponent">Type of component to retrieve.</typeparam>
        /// <returns>Component of specified type.</returns>
        public TComponent GetComponent<TComponent>() where TComponent : Component
        {
            if (_componentsByType.TryGetValue(typeof(TComponent), out var componentsOfType))
            {
                if (componentsOfType.Count != 1)
                {
                    throw new InvalidOperationException("ERR");
                }

                return (TComponent)componentsOfType[0];
            }

            throw new InvalidOperationException("ERR");
        }

        public TComponent GetComponentLoop<TComponent>() where TComponent : Component
        {
            TComponent? result = null;
            foreach (var component in _components)
            {
                if (component is TComponent targetComponent)
                {
                    if (result is not null)
                    {
                        throw new InvalidOperationException("ERR");
                    }

                    result = targetComponent;
                }
            }

            if (result is null)
            {
                throw new InvalidOperationException("ERR");
            }

            return result;
        }

        public TComponent GetComponentBaseline<TComponent>() where TComponent : Component =>
            _components.OfType<TComponent>().Single();

        /// <summary>
        ///     Returns components of specified type.
        /// </summary>
        /// <typeparam name="TComponent">Type of components to retrieve.</typeparam>
        /// <returns>Components of specified type.</returns>
        public IEnumerable<TComponent> GetComponents<TComponent>() where TComponent : Component
        {
            return _componentsByType.TryGetValue(typeof(TComponent), out var componentsOfType)
                ? componentsOfType.Cast<TComponent>()
                : Enumerable.Empty<TComponent>();
        }

        public IEnumerable<TComponent> GetComponentsLoop<TComponent>() where TComponent : Component
        {
            var list = new List<TComponent>();

            foreach (var component in _components)
            {
                if (component is TComponent targetComponent)
                {
                    list.Add(targetComponent);
                }
            }

            return list;
        }

        public IEnumerable<TComponent> GetComponentsBaseline<TComponent>() where TComponent : Component =>
            _components.OfType<TComponent>();

        /// <summary>
        ///     Checks if component of specified type is attached to entity.
        /// </summary>
        /// <typeparam name="TComponent">Type of component to check.</typeparam>
        /// <returns>True if component of specified type is attached to entity; false otherwise.</returns>
        public bool HasComponent<TComponent>() where TComponent : Component
        {
            if (_componentsByType.TryGetValue(typeof(TComponent), out var componentsOfType))
            {
                return componentsOfType.Count > 0;
            }

            return false;
        }

        public bool HasComponentLoop<TComponent>() where TComponent : Component
        {
            foreach (var component in _components)
            {
                if (component is TComponent) return true;
            }

            return false;
        }

        public bool HasComponentBaseline<TComponent>() where TComponent : Component => _components.OfType<TComponent>().Any();

        /// <summary>
        ///     Marks entity as scheduled for removal from the scene. It will be removed from scene after completing fixed time
        ///     step.
        /// </summary>
        /// <remarks>
        ///     Entities scheduled for removal with this method live until end of current fixed time step and then they are removed
        ///     from scene. This method is useful when you want to guarantee that for subsequent fixed time step this entity no
        ///     longer exists in the scene.
        /// </remarks>
        public void RemoveAfterFixedTimeStep()
        {
            ThrowIfEntityIsRemovedFromTheScene();

            Scene.MarkEntityToBeRemovedAfterFixedTimeStep(this);
        }

        /// <summary>
        ///     Marks entity as scheduled for removal from the scene. It will be removed from scene after completing current frame.
        /// </summary>
        /// <remarks>
        ///     Entities scheduled for removal with this method live until end of current frame and then they are removed from
        ///     scene. This method is useful when you want to guarantee that for the next frame this entity no longer exists in the
        ///     scene.
        /// </remarks>
        public void RemoveAfterFullFrame()
        {
            ThrowIfEntityIsRemovedFromTheScene();

            Scene.MarkEntityToBeRemovedAfterFullFrame(this);
        }

        private void ThrowIfEntityIsRemovedFromTheScene()
        {
            if (IsRemoved)
            {
                throw new InvalidOperationException("Cannot access entity that is already removed from the scene.");
            }
        }
    }
}