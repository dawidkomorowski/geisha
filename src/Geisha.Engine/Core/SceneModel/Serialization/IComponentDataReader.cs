namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentDataReader
    {
        int ReadIntProperty(string propertyName);
        double ReadDoubleProperty(string propertyName);
        string? ReadStringProperty(string propertyName);
    }
}