using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentDataReader
    {
        bool IsNullProperty(string propertyName);
        bool ReadBoolProperty(string propertyName);
        int ReadIntProperty(string propertyName);
        double ReadDoubleProperty(string propertyName);
        string? ReadStringProperty(string propertyName);
        TEnum ReadEnumProperty<TEnum>(string propertyName) where TEnum : struct;
        Vector2 ReadVector2Property(string propertyName);
        Vector3 ReadVector3Property(string propertyName);
        AssetId ReadAssetIdProperty(string propertyName);
    }
}