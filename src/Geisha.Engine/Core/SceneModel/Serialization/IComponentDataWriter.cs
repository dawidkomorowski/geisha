using Geisha.Common.Math;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentDataWriter
    {
        void WriteBoolProperty(string propertyName, bool value);
        void WriteIntProperty(string propertyName, int value);
        void WriteDoubleProperty(string propertyName, double value);
        void WriteStringProperty(string propertyName, string? value);
        void WriteEnumProperty<TEnum>(string propertyName, TEnum value) where TEnum : struct;
        void WriteVector2Property(string propertyName, Vector2 value);
        void WriteVector3Property(string propertyName, Vector3 value);
    }
}