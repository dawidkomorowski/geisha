using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Audio;

namespace Geisha.Framework.Audio.CSCore
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Framework Audio CSCore";
        public string Description => "Provides audio backend.";
        public string Category => "Audio";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AudioProvider>().As<IAudioProvider>().SingleInstance();
        }
    }
}