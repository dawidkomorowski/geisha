using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Core.SceneModel.Serialization
{
    /// <summary>
    ///     Provides API to read object data during deserialization process.
    /// </summary>
    public interface IObjectReader
    {
        /// <summary>
        ///     Checks if property of specified name is defined in object.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><c>true</c> if property is defined in object; otherwise <c>false</c>.</returns>
        bool IsDefined(string propertyName);

        /// <summary>
        ///     Checks if property of specified name has <c>null</c> value.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><c>true</c> if property has null value; otherwise <c>false</c>.</returns>
        bool IsNull(string propertyName);

        /// <summary>
        ///     Reads <see cref="bool" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="bool" /> value of property being read.</returns>
        bool ReadBool(string propertyName);

        /// <summary>
        ///     Reads <see cref="int" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="int" /> value of property being read.</returns>
        int ReadInt(string propertyName);

        /// <summary>
        ///     Reads <see cref="double" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="double" /> value of property being read.</returns>
        double ReadDouble(string propertyName);

        /// <summary>
        ///     Reads <see cref="string" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="string" /> value of property being read.</returns>
        string? ReadString(string propertyName);

        /// <summary>
        ///     Reads <see cref="Enum" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <typeparam name="TEnum">Type of <see cref="Enum" /> to read.</typeparam>
        /// <returns><typeparamref name="TEnum" /> value of property being read.</returns>
        TEnum ReadEnum<TEnum>(string propertyName) where TEnum : Enum;

        /// <summary>
        ///     Reads <see cref="Vector2" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="Vector2" /> value of property being read.</returns>
        Vector2 ReadVector2(string propertyName);

        /// <summary>
        ///     Reads <see cref="Vector3" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="Vector3" /> value of property being read.</returns>
        Vector3 ReadVector3(string propertyName);

        /// <summary>
        ///     Reads <see cref="AssetId" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="AssetId" /> value of property being read.</returns>
        AssetId ReadAssetId(string propertyName);

        /// <summary>
        ///     Reads <see cref="Color" /> property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="Color" /> value of property being read.</returns>
        Color ReadColor(string propertyName);

        /// <summary>
        ///     Reads object property of specified name with value described by <paramref name="readFunc" />.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <param name="readFunc">Function describing how to read an object of type <typeparamref name="T" />.</param>
        /// <typeparam name="T">Type of object to read.</typeparam>
        /// <returns><typeparamref name="T" /> value of property being read.</returns>
        T ReadObject<T>(string propertyName, Func<IObjectReader, T> readFunc);

        /// <summary>
        ///     Gets <see cref="IEnumerable{T}" /> enumerating all property names of object property of specified name.
        /// </summary>
        /// <param name="propertyName">Name of property to read.</param>
        /// <returns><see cref="IEnumerable{T}" /> enumerating names of all properties of object.</returns>
        IEnumerable<string> EnumerateObject(string propertyName);
    }

    /// <summary>
    ///     Provides API to read component data during deserialization process.
    /// </summary>
    public interface IComponentDataReader : IObjectReader
    {
    }
}