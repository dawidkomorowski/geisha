using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Core.SceneModel.Serialization;

/// <summary>
///     Provides methods to read object data during deserialization.
/// </summary>
public interface IObjectReader
{
    /// <summary>
    ///     Checks whether the property with the specified name is defined in the object.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns><c>true</c> if the property is defined in the object; otherwise <c>false</c>.</returns>
    bool IsDefined(string propertyName);

    /// <summary>
    ///     Checks whether the property with the specified name has a <c>null</c> value.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns><c>true</c> if the property has a null value; otherwise <c>false</c>.</returns>
    bool IsNull(string propertyName);

    /// <summary>
    ///     Reads the <see cref="bool" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>The <see cref="bool" /> value of the property.</returns>
    bool ReadBool(string propertyName);

    /// <summary>
    ///     Reads the <see cref="int" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>The <see cref="int" /> value of the property.</returns>
    int ReadInt(string propertyName);

    /// <summary>
    ///     Reads the <see cref="uint" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>The <see cref="uint" /> value of the property.</returns>
    uint ReadUInt(string propertyName);

    /// <summary>
    ///     Reads the <see cref="double" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>The <see cref="double" /> value of the property.</returns>
    double ReadDouble(string propertyName);

    /// <summary>
    ///     Reads the <see cref="string" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>
    ///     The <see cref="string" /> value of the property, or <see langword="null" /> when the property value is
    ///     <c>null</c>.
    /// </returns>
    string? ReadString(string propertyName);

    /// <summary>
    ///     Reads the <see cref="Enum" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <typeparam name="TEnum">Type of <see cref="Enum" /> to read.</typeparam>
    /// <returns>The <typeparamref name="TEnum" /> value of the property.</returns>
    TEnum ReadEnum<TEnum>(string propertyName) where TEnum : Enum;

    /// <summary>
    ///     Reads the <see cref="Vector2" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>The <see cref="Vector2" /> value of the property.</returns>
    Vector2 ReadVector2(string propertyName);

    /// <summary>
    ///     Reads the <see cref="Vector3" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>The <see cref="Vector3" /> value of the property.</returns>
    Vector3 ReadVector3(string propertyName);

    /// <summary>
    ///     Reads the <see cref="AssetId" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>The <see cref="AssetId" /> value of the property.</returns>
    AssetId ReadAssetId(string propertyName);

    /// <summary>
    ///     Reads the <see cref="Color" /> property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>The <see cref="Color" /> value of the property.</returns>
    Color ReadColor(string propertyName);

    /// <summary>
    ///     Reads the object property with the specified name using <paramref name="readFunc" />.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <param name="readFunc">Function that defines how to read an object of type <typeparamref name="T" />.</param>
    /// <typeparam name="T">Type of object to read.</typeparam>
    /// <returns>The <typeparamref name="T" /> value of the property.</returns>
    T ReadObject<T>(string propertyName, Func<IObjectReader, T> readFunc);

    /// <summary>
    ///     Enumerates names of all properties of the object property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>An enumerable sequence of property names.</returns>
    IEnumerable<string> EnumerateObject(string propertyName);
}

/// <summary>
///     Provides methods to read component data during deserialization.
/// </summary>
public interface IComponentDataReader : IObjectReader
{
}