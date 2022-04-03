namespace Geisha.Engine.Core.SceneModel
{
    // TODO Add documentation.
    public interface ISceneObserver
    {
        void OnEntityCreated(Entity entity);
        void OnEntityRemoved(Entity entity);
        void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent);
        void OnComponentCreated(Component component);
        void OnComponentRemoved(Component component);
    }
}