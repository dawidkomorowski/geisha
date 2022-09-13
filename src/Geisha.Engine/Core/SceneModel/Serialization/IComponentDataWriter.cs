using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Provides API to write object data during serialization process.
    /// </summary>
    public interface IObjectWriter
    {
        /// <summary>
        ///     Writes property with specified name and value of <c>null</c>.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        void WriteNull(string propertyName);

        /// <summary>
        ///     Writes <see cref="bool" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        void WriteBool(string propertyName, bool value);

        /// <summary>
        ///     Writes <see cref="int" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        void WriteInt(string propertyName, int value);

        /// <summary>
        ///     Writes <see cref="double" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        void WriteDouble(string propertyName, double value);

        /// <summary>
        ///     Writes <see cref="string" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        void WriteString(string propertyName, string? value);

        /// <summary>
        ///     Writes <see cref="Enum" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        /// <typeparam name="TEnum">Type of <see cref="Enum" /> to write.</typeparam>
        void WriteEnum<TEnum>(string propertyName, TEnum value) where TEnum : Enum;

        /// <summary>
        ///     Writes <see cref="Vector2" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        void WriteVector2(string propertyName, Vector2 value);

        /// <summary>
        ///     Writes <see cref="Vector3" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        void WriteVector3(string propertyName, Vector3 value);

        /// <summary>
        ///     Writes <see cref="AssetId" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        void WriteAssetId(string propertyName, AssetId value);

        /// <summary>
        ///     Writes <see cref="Color" /> property with specified name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        void WriteColor(string propertyName, Color value);

        /// <summary>
        ///     Writes object property with specified name and value described by <paramref name="writeAction" />.
        /// </summary>
        /// <param name="propertyName">Name of property to write.</param>
        /// <param name="value">Value of property to write.</param>
        /// <param name="writeAction">Action describing how to write an object of type <typeparamref name="T" />.</param>
        /// <typeparam name="T">Type of object to write.</typeparam>
        void WriteObject<T>(string propertyName, T value, Action<T, IObjectWriter> writeAction);
    }

    /// <summary>
    ///     Provides API to write component data during serialization process.
    /// </summary>
    public interface IComponentDataWriter : IObjectWriter
    {
    }
}