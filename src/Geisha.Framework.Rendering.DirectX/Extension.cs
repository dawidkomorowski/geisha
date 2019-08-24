using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Rendering;

namespace Geisha.Framework.Rendering.DirectX
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Framework Rendering DirectX";
        public string Description => "Provides DirectX rendering backend.";
        public string Category => "Rendering";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Renderer2D>().As<IRenderer2D>().SingleInstance();
        }
    }
}