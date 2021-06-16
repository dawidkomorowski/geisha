﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Represents unique identifier of component class (a class extending <see cref="Component" />).
    /// </summary>
    /// <remarks>
    ///     <see cref="ComponentId" /> is used in scene serialization to store component type information in a way
    ///     insensitive to actual component type or namespace. Default value of <see cref="ComponentId" /> is empty
    ///     <c>string</c>.
    /// </remarks>
    public readonly struct ComponentId : IEquatable<ComponentId>
    {
        private static readonly Dictionary<Type, ComponentId> ComponentIdCache = new Dictionary<Type, ComponentId>();
        private readonly string? _value;

        /// <summary>
        ///     Creates new instance of <see cref="ComponentId" /> struct given <c>string</c> identifier.
        /// </summary>
        /// <param name="value">Identifier of component class.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value" /> is <c>null</c>.</exception>
        public ComponentId(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     Value of identifier.
        /// </summary>
        public string Value => _value ?? string.Empty;

        /// <summary>
        ///     Returns a <c>string</c> representing the value of the current <see cref="ComponentId" />.
        /// </summary>
        /// <returns>A <c>string</c> representing the value of the current <see cref="ComponentId" />.</returns>
        public override string ToString() => Value;

        /// <summary>
        ///     Returns a value indicating whether the value of this instance is equal to the value of the specified
        ///     <see cref="ComponentId" /> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="other" /> parameter equals the value of this instance; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(ComponentId other) => Value == other.Value;

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is an instance of <see cref="ComponentId" /> and equals the value of this
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj) => obj is ComponentId other && Equals(other);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        ///     Determines whether two specified instances of <see cref="ComponentId" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="ComponentId" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ComponentId left, ComponentId right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="ComponentId" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="ComponentId" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ComponentId left, ComponentId right) => !left.Equals(right);

        /// <summary>
        ///     Returns <see cref="ComponentId" /> of specified <paramref name="componentType" />.
        /// </summary>
        /// <param name="componentType"><see cref="Type" /> representing component.</param>
        /// <returns><see cref="ComponentId" /> of specified <paramref name="componentType" />.</returns>
        /// <exception cref="InvalidOperationException">Thrown when FullName of <paramref name="componentType" /> is null.</exception>
        public static ComponentId Of(Type componentType)
        {
            if (ComponentIdCache.TryGetValue(componentType, out var componentId)) return componentId;

            var attribute = componentType.GetCustomAttribute<ComponentIdAttribute>(false);
            componentId = attribute?.ComponentId ??
                          new ComponentId(componentType.FullName ?? throw new InvalidOperationException("FullName of component type is null."));
            ComponentIdCache.Add(componentType, componentId);
            return componentId;
        }

        /// <summary>
        ///     Returns <see cref="ComponentId" /> of specified <typeparamref name="TComponent" />.
        /// </summary>
        /// <typeparam name="TComponent">Type of component.</typeparam>
        /// <returns><see cref="ComponentId" /> of specified <typeparamref name="TComponent" />.</returns>
        public static ComponentId Of<TComponent>() where TComponent : Component
        {
            return CachedComponentId<TComponent>.ComponentId;
        }

        private static class CachedComponentId<TComponent> where TComponent : Component
        {
            static CachedComponentId()
            {
                ComponentId = Of(typeof(TComponent));
            }

            // ReSharper disable once StaticMemberInGenericType
            public static ComponentId ComponentId { get; }
        }
    }

    /// <summary>
    ///     Defines custom <see cref="ComponentId" /> for component class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ComponentIdAttribute : Attribute
    {
        /// <summary>
        ///     Initializes new instance of <see cref="ComponentIdAttribute" />.
        /// </summary>
        /// <param name="componentId"><see cref="string" /> representing <see cref="ComponentId" />.</param>
        public ComponentIdAttribute(string componentId)
        {
            ComponentId = new ComponentId(componentId);
        }

        /// <summary>
        ///     <see cref="ComponentId" /> value of this attribute instance.
        /// </summary>
        public ComponentId ComponentId { get; }
    }
}