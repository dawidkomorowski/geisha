using Autofac;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine
{
    /// <summary>
    ///     Provides API for registering game components, systems and other services to be available for engine to use.
    /// </summary>
    /// <remarks>
    ///     Prefer to use dedicated methods for registering implementations of engine defined interfaces for example use
    ///     <see cref="RegisterSystem{TCustomSystem}" /> in order to register custom implementation of
    ///     <see cref="ICustomSystem" />. If you need more control over configuration of components registration you can access
    ///     underlying Autofac container builder through <see cref="AutofacContainerBuilder" /> property.
    /// </remarks>
    public interface IComponentsRegistry
    {
        /// <summary>
        ///     Autofac container builder used to configure components registration.
        /// </summary>
        ContainerBuilder AutofacContainerBuilder { get; }

        /// <summary>
        ///     Registers component factory of specified type in order to make corresponding component type available to the
        ///     engine.
        /// </summary>
        /// <typeparam name="TComponentFactory">Type of component factory implementation to be registered.</typeparam>
        void RegisterComponentFactory<TComponentFactory>() where TComponentFactory : IComponentFactory;

        /// <summary>
        ///     Registers scene behavior factory of specified type.
        /// </summary>
        /// <typeparam name="TSceneBehaviorFactory">Type of scene behavior factory implementation to be registered.</typeparam>
        void RegisterSceneBehaviorFactory<TSceneBehaviorFactory>() where TSceneBehaviorFactory : ISceneBehaviorFactory;

        /// <summary>
        ///     Registers custom system of specified type.
        /// </summary>
        /// <typeparam name="TCustomSystem">Type of custom system implementation to be registered.</typeparam>
        void RegisterSystem<TCustomSystem>() where TCustomSystem : ICustomSystem;
    }

    internal sealed class ComponentsRegistry : IComponentsRegistry
    {
        public ComponentsRegistry(ContainerBuilder containerBuilder)
        {
            AutofacContainerBuilder = containerBuilder;
        }

        public ContainerBuilder AutofacContainerBuilder { get; }

        public void RegisterComponentFactory<TComponentFactory>() where TComponentFactory : IComponentFactory
        {
            AutofacContainerBuilder.RegisterType<TComponentFactory>().As<IComponentFactory>().SingleInstance();
        }

        public void RegisterSceneBehaviorFactory<TSceneBehaviorFactory>() where TSceneBehaviorFactory : ISceneBehaviorFactory
        {
            AutofacContainerBuilder.RegisterType<TSceneBehaviorFactory>().As<ISceneBehaviorFactory>().SingleInstance();
        }

        public void RegisterSystem<TCustomSystem>() where TCustomSystem : ICustomSystem
        {
            AutofacContainerBuilder.RegisterType<TCustomSystem>().As<ICustomSystem>().SingleInstance();
        }
    }
}