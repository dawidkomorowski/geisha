﻿using Autofac;
using Geisha.Engine.Animation.Assets;
using Geisha.Engine.Animation.Components.Serialization;
using Geisha.Engine.Animation.Systems;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Animation
{
    /// <summary>
    ///     Provides animation system and related components.
    /// </summary>
    public sealed class AnimationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Assets
            builder.RegisterType<SpriteAnimationAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
            builder.RegisterType<SpriteAnimationManagedAssetFactory>().As<IManagedAssetFactory>().SingleInstance();

            // Components
            builder.RegisterType<SerializableSpriteAnimationComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Systems
            builder.RegisterType<AnimationSystem>().As<IAnimationSystem>().SingleInstance();
        }
    }
}