namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentDataReader
    {
        bool ReadBoolProperty(string propertyName);
        int ReadIntProperty(string propertyName);
        double ReadDoubleProperty(string propertyName);
        string? ReadStringProperty(string propertyName);
        TEnum ReadEnumProperty<TEnum>(string propertyName) where TEnum : struct;
    }
}