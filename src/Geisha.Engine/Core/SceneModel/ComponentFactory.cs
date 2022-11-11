using System;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     <see cref="IComponentFactory" /> interface defines API for implementing component factories to allow the engine to
    ///     instantiate concrete components.
    /// </summary>
    /// <remarks>
    ///     To make particular component type available to the engine create class implementing
    ///     <see cref="IComponentFactory" /> interface for this type of component, and then register component factory in
    ///     <see cref="Game.RegisterComponents" /> using
    ///     <see cref="IComponentsRegistry.RegisterComponentFactory{TComponentFactory}" />. For ease of implementation use
    ///     <see cref="ComponentFactory{TComponent}" /> as a base class. It provides implementation of the
    ///     <see cref="IComponentFactory" /> interface and improves type safety.
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
        ///     Creates new component instance of type corresponding to this component factory, attached to specified entity.
        /// </summary>
        /// <param name="entity">Entity to which new component is attached.</param>
        /// <returns>New component instance of type corresponding to this component factory.</returns>
        Component Create(Entity entity);
    }

    /// <summary>
    ///     Abstract base class simplifying implementation of <see cref="IComponentFactory" /> interface. Derive from this
    ///     class instead of directly implementing <see cref="IComponentFactory" />.
    /// </summary>
    /// <typeparam name="TComponent">Type of <see cref="Component" /> this factory creates.</typeparam>
    public abstract class ComponentFactory<TComponent> : IComponentFactory where TComponent : Component
    {
        /// <inheritdoc />
        public Type ComponentType { get; } = typeof(TComponent);

        /// <inheritdoc />
        public ComponentId ComponentId { get; } = ComponentId.Of<TComponent>();

        /// <inheritdoc />
        public Component Create(Entity entity) => CreateComponent(entity);

        /// <summary>
        ///     Creates new component instance attached to specified entity.
        /// </summary>
        /// <param name="entity">Entity to which new component is attached.</param>
        /// <returns>New component instance.</returns>
        protected abstract TComponent CreateComponent(Entity entity);
    }
}