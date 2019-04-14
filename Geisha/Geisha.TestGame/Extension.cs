﻿using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.TestGame
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Test Game";
        public string Description => "Game implementation for testing the engine.";
        public string Category => "Game";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TestSystem>().As<IFixedTimeStepSystem>().SingleInstance();
            containerBuilder.RegisterType<TestConstructionScript>().As<ISceneConstructionScript>().SingleInstance();
        }
    }
}