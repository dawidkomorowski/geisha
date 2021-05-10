namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentDataReader
    {
        string? ReadStringProperty(string propertyName);
    }
}