using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Core.SceneModel.Serialization;

/// <summary>
///     Provides methods to write object data during serialization.
/// </summary>
public interface IObjectWriter
{
    /// <summary>
    ///     Writes a property with the specified name and a value of <c>null</c>.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    void WriteNull(string propertyName);

    /// <summary>
    ///     Writes a <see cref="bool" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteBool(string propertyName, bool value);

    /// <summary>
    ///     Writes an <see cref="int" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteInt(string propertyName, int value);

    /// <summary>
    ///     Writes a <see cref="uint" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteUInt(string propertyName, uint value);

    /// <summary>
    ///     Writes a <see cref="double" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteDouble(string propertyName, double value);

    /// <summary>
    ///     Writes a <see cref="string" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteString(string propertyName, string? value);

    /// <summary>
    ///     Writes an <see cref="Enum" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    /// <typeparam name="TEnum">Type of <see cref="Enum" /> to write.</typeparam>
    void WriteEnum<TEnum>(string propertyName, TEnum value) where TEnum : Enum;

    /// <summary>
    ///     Writes a <see cref="Vector2" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteVector2(string propertyName, Vector2 value);

    /// <summary>
    ///     Writes a <see cref="Vector3" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteVector3(string propertyName, Vector3 value);

    /// <summary>
    ///     Writes an <see cref="AssetId" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteAssetId(string propertyName, AssetId value);

    /// <summary>
    ///     Writes a <see cref="Color" /> property with the specified name and value.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    void WriteColor(string propertyName, Color value);

    /// <summary>
    ///     Writes the object property with the specified name and value using <paramref name="writeAction" />.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="value">Value of the property to write.</param>
    /// <param name="writeAction">Action that defines how to write an object of type <typeparamref name="T" />.</param>
    /// <typeparam name="T">Type of object to write.</typeparam>
    void WriteObject<T>(string propertyName, T value, Action<T, IObjectWriter> writeAction);
}

/// <summary>
///     Provides methods to write component data during serialization.
/// </summary>
public interface IComponentDataWriter : IObjectWriter
{
}