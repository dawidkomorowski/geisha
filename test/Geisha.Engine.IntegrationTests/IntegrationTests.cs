using System.Drawing;
using Autofac;
using Geisha.Common;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.CSCore;
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
            var renderingConfigurationBuilder = RenderingConfiguration.CreateBuilder();
            ConfigureRendering(renderingConfigurationBuilder);
            var renderingConfiguration = renderingConfigurationBuilder.Build();

            _renderForm = new RenderForm("IntegrationTestsWindow")
                { ClientSize = new Size(renderingConfiguration.ScreenWidth, renderingConfiguration.ScreenWidth) };

            if (ShowDebugWindow) _renderForm.Show();

            var containerBuilder = new ContainerBuilder();

            // Register configuration
            containerBuilder.RegisterInstance(renderingConfiguration).As<RenderingConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(PhysicsConfiguration.CreateBuilder().Build()).As<PhysicsConfiguration>().SingleInstance();

            // Register engine back-ends
            containerBuilder.RegisterInstance(new CSCoreAudioBackend()).As<IAudioBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new WindowsInputBackend(_renderForm)).As<IInputBackend>().SingleInstance();
            containerBuilder.RegisterInstance(new DirectXRenderingBackend(_renderForm, DriverType.Software)).As<IRenderingBackend>().SingleInstance();

            // Register common modules
            CommonModules.RegisterAll(containerBuilder);

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

        protected virtual void ConfigureRendering(RenderingConfiguration.IBuilder builder)
        {
            builder
                .WithScreenWidth(1280)
                .WithScreenHeight(720)
                .WithEnableVSync(false)
                .WithSortingLayersOrder(new[] { RenderingConfiguration.DefaultSortingLayerName });
        }

        protected virtual void RegisterTestComponents(ContainerBuilder containerBuilder)
        {
        }
    }
}