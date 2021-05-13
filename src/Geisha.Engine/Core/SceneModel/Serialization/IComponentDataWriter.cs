using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    public interface IObjectWriter
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
        void WriteColor(string propertyName, Color value);
        void WriteObject<T>(string propertyName, T value, Action<T, IObjectWriter> writeAction);
    }

    public interface IComponentDataWriter : IObjectWriter
    {
    }
}