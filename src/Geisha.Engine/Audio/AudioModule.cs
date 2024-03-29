﻿using Autofac;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Audio
{
    internal sealed class AudioModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Assets
            builder.RegisterType<SoundAssetLoader>().As<IAssetLoader>().SingleInstance();

            // Components
            builder.RegisterType<AudioSourceComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Systems
            builder.RegisterType<AudioSystem>().As<IAudioGameLoopStep>().As<ISceneObserver>().SingleInstance();
        }
    }
}