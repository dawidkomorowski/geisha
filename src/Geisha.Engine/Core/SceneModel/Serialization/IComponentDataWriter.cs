using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IComponentDataWriter
    {
        void WriteNull(string propertyName);
        void WriteBool(string propertyName, bool value);
        void WriteInt(string propertyName, int value);
        void WriteDouble(string propertyName, double value);
        void WriteString(string propertyName, string? value);
        void WriteEnum<TEnum>(string propertyName, TEnum value) where TEnum : struct;
        void WriteVector2(string propertyName, Vector2 value);
        void WriteVector3(string propertyName, Vector3 value);
        void WriteAssetId(string propertyName, AssetId value);
    }
}