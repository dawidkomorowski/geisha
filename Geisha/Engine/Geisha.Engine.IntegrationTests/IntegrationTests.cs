using System;
using Autofac;
using Geisha.Common.Extensibility;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests
{
    public abstract class IntegrationTests<TSystemUnderTest>
    {
        private ExtensionsHostContainer<TSystemUnderTest> _extensionsHostContainer;
        protected TSystemUnderTest SystemUnderTest { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            _extensionsHostContainer = new ExtensionsHostContainer<TSystemUnderTest>(new HostServices(RegisterComponents));
            SystemUnderTest = _extensionsHostContainer.CompositionRoot;
        }

        [TearDown]
        public virtual void TearDown()
        {
            _extensionsHostContainer.Dispose();
        }

        protected virtual void RegisterComponents(ContainerBuilder containerBuilder)
        {
        }

        private sealed class HostServices : IHostServices
        {
            private readonly Action<ContainerBuilder> _registerComponents;

            public HostServices(Action<ContainerBuilder> registerComponents)
            {
                _registerComponents = registerComponents;
            }

            public void Register(ContainerBuilder containerBuilder)
            {
                containerBuilder.RegisterType<TSystemUnderTest>().AsSelf().SingleInstance();
                _registerComponents(containerBuilder);
            }
        }
    }
}