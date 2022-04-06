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
        private readonly IComponentFactoryProvider _componentFactoryProvider;
        private readonly List<Entity> _entities = new List<Entity>(); // TODO Would HashSet be faster?
        private readonly List<Entity> _rootEntities = new List<Entity>(); // TODO Would HashSet be faster?
        private readonly List<Entity> _entitiesToRemoveAfterFixedTimeStep = new List<Entity>();
        private readonly List<Entity> _entitiesToRemoveAfterFullFrame = new List<Entity>();
        private readonly List<ISceneObserver> _observers = new List<ISceneObserver>();

        /// <summary>
        ///     Creates new instance of <see cref="Scene" /> class.
        /// </summary>
        internal Scene(IComponentFactoryProvider componentFactoryProvider)
        {
            _componentFactoryProvider = componentFactoryProvider;
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
            var entity = new Entity(this, _componentFactoryProvider);
            _entities.Add(entity);
            _rootEntities.Add(entity);

            NotifyEntityCreated(entity);

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

            while (entity.Components.Count != 0)
            {
                entity.RemoveComponent(entity.Components[0]);
            }

            entity.Parent = null;
            _entities.Remove(entity);
            _rootEntities.Remove(entity);
            entity.IsRemoved = true;

            NotifyEntityRemoved(entity);
        }

        #region Internal API for Entity class

        /// <summary>
        ///     Internal API for <see cref="Entity" /> class.
        /// </summary>
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

            NotifyEntityParentChanged(entity, oldParent, newParent);
        }

        /// <summary>
        ///     Internal API for <see cref="Entity" /> class.
        /// </summary>
        internal void OnComponentCreated(Component component)
        {
            NotifyComponentCreated(component);
        }

        /// <summary>
        ///     Internal API for <see cref="Entity" /> class.
        /// </summary>
        internal void OnComponentRemoved(Component component)
        {
            NotifyComponentRemoved(component);
        }

        /// <summary>
        ///     Internal API for <see cref="Entity" /> class.
        /// </summary>
        internal void MarkEntityToBeRemovedAfterFixedTimeStep(Entity entity)
        {
            _entitiesToRemoveAfterFixedTimeStep.Add(entity);
        }

        /// <summary>
        ///     Internal API for <see cref="Entity" /> class.
        /// </summary>
        internal void MarkEntityToBeRemovedAfterFullFrame(Entity entity)
        {
            _entitiesToRemoveAfterFullFrame.Add(entity);
        }

        #endregion

        #region Internal API for SceneManager class

        /// <summary>
        ///     Internal API for <see cref="SceneManager" /> class.
        /// </summary>
        internal void AddObserver(ISceneObserver observer)
        {
            if (_observers.Contains(observer))
            {
                throw new ArgumentException("Observer is already added to this scene.");
            }

            _observers.Add(observer);

            foreach (var rootEntity in _rootEntities)
            {
                NotifyObserverAboutExistingEntityTree(observer, rootEntity);
            }
        }

        /// <summary>
        ///     Internal API for <see cref="SceneManager" /> class.
        /// </summary>
        internal void RemoveObserver(ISceneObserver observer)
        {
            if (_observers.Remove(observer) == false)
            {
                throw new ArgumentException("Observer to remove was not found.");
            }

            foreach (var rootEntity in _rootEntities)
            {
                NotifyObserverToRemoveEntityTree(observer, rootEntity);
            }
        }

        /// <summary>
        ///     Internal API for <see cref="SceneManager" /> class.
        /// </summary>
        internal void OnLoaded()
        {
            SceneBehavior.OnLoaded();
        }

        #endregion

        #region Internal API for GameLoop class

        /// <summary>
        ///     Internal API for <see cref="GameLoop" /> class.
        /// </summary>
        internal void RemoveEntitiesAfterFixedTimeStep()
        {
            foreach (var entity in _entitiesToRemoveAfterFixedTimeStep)
            {
                RemoveEntity(entity);
            }

            _entitiesToRemoveAfterFixedTimeStep.Clear();
        }

        /// <summary>
        ///     Internal API for <see cref="GameLoop" /> class.
        /// </summary>
        internal void RemoveEntitiesAfterFullFrame()
        {
            foreach (var entity in _entitiesToRemoveAfterFullFrame)
            {
                RemoveEntity(entity);
            }

            _entitiesToRemoveAfterFullFrame.Clear();
        }

        #endregion

        #region Observers notifications

        private void NotifyEntityCreated(Entity entity)
        {
            foreach (var observer in _observers)
            {
                observer.OnEntityCreated(entity);
            }
        }

        private void NotifyEntityRemoved(Entity entity)
        {
            foreach (var observer in _observers)
            {
                observer.OnEntityRemoved(entity);
            }
        }

        private void NotifyEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
            foreach (var observer in _observers)
            {
                observer.OnEntityParentChanged(entity, oldParent, newParent);
            }
        }

        private void NotifyComponentCreated(Component component)
        {
            foreach (var observer in _observers)
            {
                observer.OnComponentCreated(component);
            }
        }

        private void NotifyComponentRemoved(Component component)
        {
            foreach (var observer in _observers)
            {
                observer.OnComponentRemoved(component);
            }
        }

        private static void NotifyObserverAboutExistingEntityTree(ISceneObserver observer, Entity entity)
        {
            observer.OnEntityCreated(entity);

            if (!entity.IsRoot)
            {
                observer.OnEntityParentChanged(entity, null, entity.Parent);
            }

            foreach (var component in entity.Components)
            {
                observer.OnComponentCreated(component);
            }

            foreach (var child in entity.Children)
            {
                NotifyObserverAboutExistingEntityTree(observer, child);
            }
        }

        private static void NotifyObserverToRemoveEntityTree(ISceneObserver observer, Entity entity)
        {
            foreach (var child in entity.Children)
            {
                NotifyObserverToRemoveEntityTree(observer, child);
            }

            foreach (var component in entity.Components)
            {
                observer.OnComponentRemoved(component);
            }

            if (!entity.IsRoot)
            {
                observer.OnEntityParentChanged(entity, entity.Parent, null);
            }

            observer.OnEntityRemoved(entity);
        }

        #endregion
    }
}