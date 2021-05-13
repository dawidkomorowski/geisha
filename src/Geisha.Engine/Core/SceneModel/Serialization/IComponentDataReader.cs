using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IObjectReader
    {
        bool IsDefined(string propertyName);
        bool IsNull(string propertyName);
        bool ReadBool(string propertyName);
        int ReadInt(string propertyName);
        double ReadDouble(string propertyName);
        string? ReadString(string propertyName);
        TEnum ReadEnum<TEnum>(string propertyName) where TEnum : struct;
        Vector2 ReadVector2(string propertyName);
        Vector3 ReadVector3(string propertyName);
        AssetId ReadAssetId(string propertyName);
        Color ReadColor(string propertyName);
        T ReadObject<T>(string propertyName, Func<IObjectReader, T> readFunc);
    }

    public interface IComponentDataReader : IObjectReader
    {
    }
}