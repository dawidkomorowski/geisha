using System.Drawing;
using Autofac;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.CSCore;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Windows;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.DirectX;
using NUnit.Framework;
using SharpDX.Windows;

namespace Geisha.Engine.IntegrationTests
{
    public abstract class IntegrationTests<TSystemUnderTest>
    {
        private RenderForm _renderForm;
        private IContainer _container;
        private ILifetimeScope _lifetimeScope;
        protected TSystemUnderTest SystemUnderTest { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            _renderForm = new RenderForm("IntegrationTestsWindow") {ClientSize = new Size(1280, 720)};
            var containerBuilder = new ContainerBuilder();

            // Register engine back-ends
            containerBuilder.RegisterInstance(new CSCoreAudioBackend()).As<IAudioBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new WindowsInputBackend(_renderForm)).As<IInputBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new DirectXRenderingBackend(_renderForm)).As<IRenderingBackend>().SingleInstance();

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