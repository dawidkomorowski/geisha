namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Specifies interface of scene observer that is object being notified about changes in structure of currently loaded
    ///     scene.
    /// </summary>
    /// <remarks>
    ///     This interface is intended to implement systems with efficient internal scene representation, according to system's
    ///     domain, that needs to stay in sync with main scene model.
    /// </remarks>
    public interface ISceneObserver
    {
        /// <summary>
        ///     Invoked when new entity is created in the scene.
        /// </summary>
        /// <param name="entity">New entity that was created.</param>
        void OnEntityCreated(Entity entity);

        /// <summary>
        ///     Invoked when entity is removed from the scene.
        /// </summary>
        /// <param name="entity">Entity that was removed from the scene.</param>
        void OnEntityRemoved(Entity entity);

        /// <summary>
        ///     Invoked when entity's parent has changed.
        /// </summary>
        /// <param name="entity">Entity of which parent has changed.</param>
        /// <param name="oldParent">Old parent of entity.</param>
        /// <param name="newParent">New parent of entity.</param>
        void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent);

        /// <summary>
        ///     Invoked when new component is created for entity.
        /// </summary>
        /// <param name="component">New component that was created.</param>
        void OnComponentCreated(Component component);

        /// <summary>
        ///     Invoked when component is removed from entity.
        /// </summary>
        /// <param name="component">Component that was removed from entity.</param>
        void OnComponentRemoved(Component component);
    }
}