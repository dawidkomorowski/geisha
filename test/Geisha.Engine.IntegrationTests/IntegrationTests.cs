using System.Drawing;
using Autofac;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.NAudio;
using Geisha.Engine.Core;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Input.Windows;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;
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
        protected virtual bool ShowDebugWindow => false;

        [SetUp]
        public virtual void SetUp()
        {
            var renderingConfiguration = ConfigureRendering(new RenderingConfiguration());

            var screenSize = renderingConfiguration.ScreenSize;
            _renderForm = new RenderForm("IntegrationTestsWindow")
            {
                ClientSize = new Size(screenSize.Width, screenSize.Height)
            };

            if (ShowDebugWindow) _renderForm.Show();

            var containerBuilder = new ContainerBuilder();

            // Register configuration
            containerBuilder.RegisterInstance(new CoreConfiguration()).As<CoreConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(renderingConfiguration).As<RenderingConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(new PhysicsConfiguration()).As<PhysicsConfiguration>().SingleInstance();

            // Register engine back-ends
            containerBuilder.RegisterInstance(new NAudioAudioBackend()).As<IAudioBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new WindowsInputBackend(_renderForm)).As<IInputBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new DirectXRenderingBackend(_renderForm, DriverType.Software)).As<IRenderingBackend>().SingleInstance();

            // Register engine modules
            EngineModules.RegisterAll(containerBuilder);

            // Register test components
            RegisterTestComponents(containerBuilder);

            // Register tested components
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

        protected virtual RenderingConfiguration ConfigureRendering(RenderingConfiguration configuration)
        {
            return configuration;
        }

        protected virtual void RegisterTestComponents(ContainerBuilder containerBuilder)
        {
        }
    }
}