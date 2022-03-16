﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Entity represents any object in the game scene and it's behavior and interactions are defined by attached
    ///     components processed by systems.
    /// </summary>
    public sealed class Entity
    {
        private readonly List<Entity> _children = new List<Entity>();
        private readonly List<Component> _components = new List<Component>();
        private Entity? _parent;

        /// <summary>
        ///     Should only be used by <see cref="SceneModel.Scene" />.
        /// </summary>
        internal Entity(Scene scene)
        {
            Scene = scene;
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
        public bool IsRoot => Parent == null;

        /// <summary>
        ///     Root of entity tree that this entity is part of. Returns this entity if it is root of entity tree.
        /// </summary>
        public Entity Root => IsRoot ? this : Parent!.Root;

        /// <summary>
        ///     Entities that are children of this entity.
        /// </summary>
        public IReadOnlyList<Entity> Children => _children.AsReadOnly();

        /// <summary>
        ///     Components attached to this entity.
        /// </summary>
        public IReadOnlyList<Component> Components => _components.AsReadOnly();

        /// <summary>
        ///     Indicates if entity is scheduled for destruction. Mark entity for destruction with
        ///     <see cref="DestroyAfterFixedTimeStep" /> and <see cref="DestroyAfterFullFrame" />.
        /// </summary>
        public bool IsScheduledForDestruction => DestructionTime != DestructionTime.Never;

        internal DestructionTime DestructionTime { get; private set; } = DestructionTime.Never;

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
            return Children.SelectMany(c => c.GetChildrenRecursively()).Concat(Children); // TODO This can be very expensive.
        }

        /// <summary>
        ///     Returns this entity and all its children including children of children effectively iterating through whole
        ///     subtree.
        /// </summary>
        /// <returns>Entities collection that contains this entity and all its children including children of children.</returns>
        public IEnumerable<Entity> GetChildrenRecursivelyIncludingRoot()
        {
            return Children.SelectMany(c => c.GetChildrenRecursivelyIncludingRoot()).Concat(new[] { this }); // TODO This is very expensive.
        }

        /// <summary>
        ///     Attaches specified component instance to entity.
        /// </summary>
        /// <param name="component">Component instance to be attached.</param>
        public void AddComponent(Component component)
        {
            ThrowIfEntityIsRemovedFromTheScene();

            _components.Add(component);
        }

        /// <summary>
        ///     Removes specified component instance from entity.
        /// </summary>
        /// <param name="component">Component instance to be removed from entity.</param>
        public void RemoveComponent(Component component)
        {
            ThrowIfEntityIsRemovedFromTheScene();

            _components.Remove(component);
        }

        /// <summary>
        ///     Returns component of specified type. Throws exception if there are multiple or none of requested components.
        /// </summary>
        /// <typeparam name="TComponent">Type of component to retrieve.</typeparam>
        /// <returns>Component of specified type.</returns>
        public TComponent GetComponent<TComponent>() where TComponent : Component =>
            _components.OfType<TComponent>().Single(); // TODO This is very inefficient.

        /// <summary>
        ///     Returns components of specified type.
        /// </summary>
        /// <typeparam name="TComponent">Type of components to retrieve.</typeparam>
        /// <returns>Components of specified type.</returns>
        public IEnumerable<TComponent> GetComponents<TComponent>() where TComponent : Component =>
            _components.OfType<TComponent>(); // TODO This is a bit inefficient.

        /// <summary>
        ///     Checks if component of specified type is attached to entity.
        /// </summary>
        /// <typeparam name="TComponent">Type of component to check.</typeparam>
        /// <returns>True if component of specified type is attached to entity; false otherwise.</returns>
        public bool HasComponent<TComponent>() where TComponent : Component => _components.OfType<TComponent>().Any(); // TODO This is very inefficient.

        // TODO Is Destroy a good name? Wouldn't Remove be more natural?
        /// <summary>
        ///     Marks entity as scheduled for destruction. It will be removed from scene after completing fixed time step.
        /// </summary>
        /// <remarks>
        ///     It can be examined if entity is scheduled for destruction by checking <see cref="IsScheduledForDestruction" />.
        ///     Entities scheduled for destruction with this method live until end of current fixed time step and then they are
        ///     removed from scene. This method is useful when you want to guarantee that for subsequent fixed time step this
        ///     entity no longer exists in the scene.
        /// </remarks>
        public void DestroyAfterFixedTimeStep()
        {
            ThrowIfEntityIsRemovedFromTheScene();

            DestructionTime = DestructionTime.AfterFixedTimeStep;
        }

        // TODO Is Destroy a good name? Wouldn't Remove be more natural?
        /// <summary>
        ///     Marks entity as scheduled for destruction. It will be removed from scene after completing current frame.
        /// </summary>
        /// <remarks>
        ///     It can be examined if entity is scheduled for destruction by checking <see cref="IsScheduledForDestruction" />.
        ///     Entities scheduled for destruction with this method live until end of current frame and then they are removed from
        ///     scene. This method is useful when you want to guarantee that for the next frame this entity no longer exists in the
        ///     scene.
        /// </remarks>
        public void DestroyAfterFullFrame()
        {
            ThrowIfEntityIsRemovedFromTheScene();

            DestructionTime = DestructionTime.AfterFullFrame;
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