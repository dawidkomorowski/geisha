﻿using Autofac;
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
        ///     Registers custom system of specified type.
        /// </summary>
        /// <typeparam name="TCustomSystem">Type of custom system implementation to be registered.</typeparam>
        void RegisterSystem<TCustomSystem>() where TCustomSystem : ICustomSystem;

        /// <summary>
        ///     Registers scene construction script of specified type.
        /// </summary>
        /// <typeparam name="TSceneConstructionScript">Type of scene construction script implementation to be registered.</typeparam>
        void RegisterSceneConstructionScript<TSceneConstructionScript>() where TSceneConstructionScript : ISceneConstructionScript;
    }

    internal sealed class ComponentsRegistry : IComponentsRegistry
    {
        public ComponentsRegistry(ContainerBuilder containerBuilder)
        {
            AutofacContainerBuilder = containerBuilder;
        }

        public ContainerBuilder AutofacContainerBuilder { get; }

        public void RegisterSystem<TCustomSystem>() where TCustomSystem : ICustomSystem
        {
            AutofacContainerBuilder.RegisterType<TCustomSystem>().As<ICustomSystem>().SingleInstance();
        }

        public void RegisterSceneConstructionScript<TSceneConstructionScript>() where TSceneConstructionScript : ISceneConstructionScript
        {
            AutofacContainerBuilder.RegisterType<TSceneConstructionScript>().As<ISceneConstructionScript>().SingleInstance();
        }
    }
}