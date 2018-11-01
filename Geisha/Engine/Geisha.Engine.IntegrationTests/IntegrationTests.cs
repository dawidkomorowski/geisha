using System;
using System.IO;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Common.Math;
using NUnit.Framework;
using NUnit.Framework.Internal;

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

        protected string TestDirectory => TestContext.CurrentContext.TestDirectory;
        protected string GetPathUnderTestDirectory(string path) => Path.Combine(TestDirectory, path);
        protected string GetRandomFilePath() => GetPathUnderTestDirectory(Path.GetRandomFileName());
        protected Randomizer Random => TestContext.CurrentContext.Random;
        protected Vector2 NewRandomVector2() => new Vector2(Random.NextDouble(), Random.NextDouble());

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