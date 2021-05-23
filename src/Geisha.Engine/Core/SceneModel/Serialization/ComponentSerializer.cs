namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentSerializer
    {
        ComponentId ComponentId { get; }

        void Serialize(Component component, IComponentDataWriter componentDataWriter);
        void Deserialize(Component component, IComponentDataReader componentDataReader);
    }

    public abstract class ComponentSerializer<TComponent> : IComponentSerializer where TComponent : Component
    {
        protected ComponentSerializer(ComponentId componentId)
        {
            ComponentId = componentId;
        }

        public ComponentId ComponentId { get; }

        public void Serialize(Component component, IComponentDataWriter componentDataWriter)
        {
            Serialize((TComponent) component, componentDataWriter);
        }

        public void Deserialize(Component component, IComponentDataReader componentDataReader)
        {
            Deserialize((TComponent) component, componentDataReader);
        }

        protected abstract void Serialize(TComponent component, IComponentDataWriter componentDataWriter);
        protected abstract void Deserialize(TComponent component, IComponentDataReader componentDataReader);
    }
}