namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentDataWriter
    {
        void WriteStringProperty(string propertyName, string? value);
    }
}