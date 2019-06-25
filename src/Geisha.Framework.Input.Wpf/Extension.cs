using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Input;

namespace Geisha.Framework.Input.Wpf
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Framework Input WPF";
        public string Description => "Provides WPF-based input backend.";
        public string Category => "Input";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<InputProvider>().As<IInputProvider>().SingleInstance();
            containerBuilder.RegisterType<KeyMapper>().As<IKeyMapper>().SingleInstance();
        }
    }
}