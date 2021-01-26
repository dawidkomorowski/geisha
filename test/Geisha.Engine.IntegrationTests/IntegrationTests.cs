using System.Drawing;
using Autofac;
using Geisha.Common;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.CSCore;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Input.Windows;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.DirectX;
using NUnit.Framework;
using SharpDX.Windows;

namespace Geisha.Engine.IntegrationTests
{
    public abstract class IntegrationTests<TSystemUnderTest> where TSystemUnderTest : notnull
    {
        private RenderForm _renderForm = null!;
        private IContainer _container = null!;
        private ILifetimeScope _lifetimeScope = null!;
        protected TSystemUnderTest SystemUnderTest { get; private set; } = default!;

        [SetUp]
        public virtual void SetUp()
        {
            _renderForm = new RenderForm("IntegrationTestsWindow") {ClientSize = new Size(1280, 720)};
            var containerBuilder = new ContainerBuilder();

            // Register engine back-ends
            containerBuilder.RegisterInstance(new CSCoreAudioBackend(false)).As<IAudioBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new WindowsInputBackend(_renderForm)).As<IInputBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new DirectXRenderingBackend(_renderForm)).As<IRenderingBackend>().SingleInstance();

            // Register common modules
            CommonModules.RegisterAll(containerBuilder);

            // Register engine modules
            EngineModules.RegisterAll(containerBuilder);

            // Register test components
            containerBuilder.RegisterType<TSystemUnderTest>().AsSelf().SingleInstance();

            _container = containerBuilder.Build();
            _lifetimeScope = _container.BeginLifetimeScope();

            SystemUnderTest = _lifetimeScope.Resolve<TSystemUnderTest>();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _lifetimeScope.Dispose();
            _container.Dispose();
            _renderForm.Dispose();
        }
    }
}