using Autofac;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.NAudio;
using Geisha.Engine.Core;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Input.Windows;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.DirectX;
using Geisha.Engine.Windowing;
using Geisha.Engine.Windowing.Backend;
using Geisha.Engine.Windowing.Windows;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests
{
    public abstract class IntegrationTests<TSystemUnderTest> where TSystemUnderTest : notnull
    {
        private IContainer _container = null!;
        protected TSystemUnderTest SystemUnderTest { get; private set; } = default!;
        protected virtual bool ShowDebugWindow => false;

        [SetUp]
        public virtual void SetUp()
        {
            var configuration = Configure(Configuration.CreateDefault());

            var containerBuilder = new ContainerBuilder();

            // Register configuration
            containerBuilder.RegisterInstance(configuration.Audio).As<AudioConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(configuration.Core).As<CoreConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(configuration.Physics).As<PhysicsConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(configuration.Rendering).As<RenderingConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(configuration.Windowing).As<WindowingConfiguration>().SingleInstance();

            // Register engine back-ends
            var windowingBackend = new WindowsWindowingBackend();
            windowingBackend.WindowTitle = "IntegrationTestsWindow";
            windowingBackend.WindowClientSize = configuration.Windowing.WindowClientSize;

            if (ShowDebugWindow) windowingBackend.Window.Show();

            containerBuilder.RegisterInstance(windowingBackend).As<IWindowingBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new NAudioAudioBackend()).As<IAudioBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new WindowsInputBackend(windowingBackend.Window)).As<IInputBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new DirectXRenderingBackend(windowingBackend.Window, DriverType.Software)).As<IRenderingBackend>()
                .SingleInstance();

            // Register engine modules
            EngineModules.RegisterAll(containerBuilder);

            // Register test components
            RegisterTestComponents(containerBuilder);

            // Register tested components
            containerBuilder.RegisterType<TSystemUnderTest>().AsSelf().SingleInstance();

            _container = containerBuilder.Build();

            SystemUnderTest = _container.Resolve<TSystemUnderTest>();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _container.Dispose();
        }

        protected virtual Configuration Configure(Configuration configuration)
        {
            return configuration;
        }

        protected virtual void RegisterTestComponents(ContainerBuilder containerBuilder)
        {
        }
    }
}