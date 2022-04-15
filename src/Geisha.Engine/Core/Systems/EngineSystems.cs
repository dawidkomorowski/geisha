using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    internal interface IAnimationSystem
    {
        void ProcessAnimations(GameTime gameTime);
    }

    internal interface IAudioSystem
    {
        void ProcessAudio();
    }

    internal interface IBehaviorSystem
    {
        void ProcessBehaviorFixedUpdate(Scene scene);
        void ProcessBehaviorUpdate(Scene scene, GameTime gameTime);
    }

    internal interface IInputSystem
    {
        void ProcessInput();
    }

    internal interface IPhysicsSystem
    {
        void ProcessPhysics(Scene scene);
        void PreparePhysicsDebugInformation();
    }

    internal interface IRenderingSystem
    {
        void RenderScene(Scene scene);
    }

    internal interface IEngineSystems
    {
        IAnimationSystem AnimationSystem { get; }
        IAudioSystem AudioSystem { get; }
        IBehaviorSystem BehaviorSystem { get; }
        IInputSystem InputSystem { get; }
        IPhysicsSystem PhysicsSystem { get; }
        IRenderingSystem RenderingSystem { get; }
        IReadOnlyCollection<ICustomSystem> CustomSystems { get; }

        string AnimationSystemName { get; }
        string AudioSystemName { get; }
        string BehaviorSystemName { get; }
        string InputSystemName { get; }
        string PhysicsSystemName { get; }
        string RenderingSystemName { get; }
        IReadOnlyCollection<string> SystemsNames { get; }
    }

    internal sealed class EngineSystems : IEngineSystems
    {
        private static readonly ILog Log = LogFactory.Create(typeof(EngineSystems));

        public EngineSystems(
            IAnimationSystem animationSystem,
            IAudioSystem audioSystem,
            IBehaviorSystem behaviorSystem,
            IInputSystem inputSystem,
            IPhysicsSystem physicsSystem,
            IRenderingSystem renderingSystem,
            IEnumerable<ICustomSystem> customSystems,
            CoreConfiguration configuration)
        {
            AnimationSystem = animationSystem;
            AudioSystem = audioSystem;
            BehaviorSystem = behaviorSystem;
            InputSystem = inputSystem;
            PhysicsSystem = physicsSystem;
            RenderingSystem = renderingSystem;

            var customSystemsExecutionOrder = configuration.CustomSystemsExecutionOrder;
            var customSystemsList = customSystems.ToList();
            var customSystemsSortedList = new List<ICustomSystem>();

            Log.Info("Searching for custom systems...");
            foreach (var customSystem in customSystemsList)
            {
                Log.Info($"Custom system found: {customSystem.Name}");
            }

            if (customSystemsExecutionOrder.Count != customSystemsExecutionOrder.Distinct().Count())
                throw new ArgumentException("Configuration specifies duplicated custom systems. Each custom system can be specified only once.");

            var customSystemsNames = customSystemsList.Select(cs => cs.Name).ToList();
            if (customSystemsNames.Count != customSystemsNames.Distinct().Count())
                throw new ArgumentException("There are custom system with duplicated names. Each system must have unique name.");

            foreach (var systemName in customSystemsExecutionOrder)
            {
                var customSystem = customSystemsList.SingleOrDefault(cs => cs.Name == systemName);
                if (customSystem == null)
                    throw new ArgumentException($"Cannot find custom system specified in configuration. Custom system name: {systemName}");

                customSystemsSortedList.Add(customSystem);
            }

            CustomSystems = customSystemsSortedList.AsReadOnly();

            SystemsNames = new[]
            {
                AnimationSystemName,
                AudioSystemName,
                BehaviorSystemName,
                InputSystemName,
                PhysicsSystemName,
                RenderingSystemName
            }.Concat(CustomSystems.Select(cs => cs.Name)).OrderBy(n => n).ToList().AsReadOnly();

            Log.Info("Custom systems has been configured to execute in following order:");
            foreach (var customSystem in CustomSystems)
            {
                Log.Info($"Custom system name: {customSystem.Name}");
            }
        }

        public IAnimationSystem AnimationSystem { get; }
        public IAudioSystem AudioSystem { get; }
        public IBehaviorSystem BehaviorSystem { get; }
        public IInputSystem InputSystem { get; }
        public IPhysicsSystem PhysicsSystem { get; }
        public IRenderingSystem RenderingSystem { get; }
        public IReadOnlyCollection<ICustomSystem> CustomSystems { get; }

        public string AnimationSystemName => nameof(AnimationSystem);
        public string AudioSystemName => nameof(AudioSystem);
        public string BehaviorSystemName => nameof(BehaviorSystem);
        public string InputSystemName => nameof(InputSystem);
        public string PhysicsSystemName => nameof(PhysicsSystem);
        public string RenderingSystemName => nameof(RenderingSystem);
        public IReadOnlyCollection<string> SystemsNames { get; }
    }
}