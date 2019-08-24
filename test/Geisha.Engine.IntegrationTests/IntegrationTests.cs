using System.Drawing;
using Autofac;
using Geisha.Common.Extensibility;
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
        // TODO Replace with raw autofac container when Geisha.Framework will no longer be as extension.
        private ExtensionsHostContainer<TSystemUnderTest> _extensionsHostContainer;
        private RenderForm _renderForm;
        protected TSystemUnderTest SystemUnderTest { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            _renderForm = new RenderForm($"IntegrationTestsWindow") {ClientSize = new Size(1280, 720)};
            _extensionsHostContainer = new ExtensionsHostContainer<TSystemUnderTest>(new HostServices());
            SystemUnderTest = _extensionsHostContainer.CompositionRoot;
        }

        [TearDown]
        public virtual void TearDown()
        {
            _extensionsHostContainer.Dispose();
            _renderForm.Dispose();
        }

        private sealed class HostServices : IHostServices
        {
            public void Register(ContainerBuilder containerBuilder)
            {
                // TODO Get rid of this not disposed form after removing HostServices.
                var renderForm = new RenderForm($"IntegrationTestsWindow") {ClientSize = new Size(1280, 720)};
                // Register engine back-ends
                containerBuilder.RegisterInstance(new CSCoreAudioBackend()).As<IAudioBackend>().SingleInstance();
                containerBuilder.RegisterInstance(new WindowsInputBackend(renderForm)).As<IInputBackend>().SingleInstance();
                containerBuilder.RegisterInstance(new DirectXRenderingBackend(renderForm)).As<IRenderingBackend>().SingleInstance();

                // Register engine modules
                EngineModules.RegisterAll(containerBuilder);

                // Register test components
                containerBuilder.RegisterType<TSystemUnderTest>().AsSelf().SingleInstance();
            }
        }
    }
}