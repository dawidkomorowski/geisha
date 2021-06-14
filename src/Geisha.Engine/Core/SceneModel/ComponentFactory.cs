﻿using System;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     <see cref="IComponentFactory" /> interface defines API for implementing component factories to allow the engine to
    ///     instantiate concrete components.
    /// </summary>
    /// <remarks>
    ///     To make a particular component type available to the engine create class implementing
    ///     <see cref="IComponentFactory" /> interface for this type of component, and then register component factory in
    ///     <see cref="IGame.RegisterComponents" /> using
    ///     <see cref="IComponentsRegistry.RegisterComponentFactory{TComponentFactory}" />.
    /// </remarks>
    public interface IComponentFactory
    {
        /// <summary>
        ///     Type of component this factory creates.
        /// </summary>
        Type ComponentType { get; }

        /// <summary>
        ///     Id of component this factory creates.
        /// </summary>
        ComponentId ComponentId { get; }

        /// <summary>
        ///     Creates new component instance of type corresponding to this component factory.
        /// </summary>
        /// <returns>New component instance of type corresponding to this component factory.</returns>
        Component Create();
    }

    public abstract class ComponentFactory<TComponent> : IComponentFactory where TComponent : Component
    {
        public Type ComponentType { get; } = typeof(TComponent);
        public ComponentId ComponentId { get; } = ComponentId.Of<TComponent>();
        public Component Create() => CreateComponent();

        protected abstract TComponent CreateComponent();
    }
}