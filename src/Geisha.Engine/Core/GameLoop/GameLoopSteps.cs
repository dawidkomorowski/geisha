using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core.GameLoop
{
    internal interface IGameLoopSteps
    {
        IAnimationGameLoopStep AnimationStep { get; }
        IAudioGameLoopStep AudioStep { get; }
        IBehaviorSystem BehaviorSystem { get; }
        IInputSystem InputSystem { get; }
        IPhysicsSystem PhysicsSystem { get; }
        IRenderingSystem RenderingSystem { get; }
        IReadOnlyCollection<ICustomSystem> CustomSystems { get; }

        string AnimationStepName { get; }
        string AudioStepName { get; }
        string BehaviorSystemName { get; }
        string InputSystemName { get; }
        string PhysicsSystemName { get; }
        string RenderingSystemName { get; }
        IReadOnlyCollection<string> SystemsNames { get; }
    }

    internal sealed class GameLoopSteps : IGameLoopSteps
    {
        private static readonly ILog Log = LogFactory.Create(typeof(GameLoopSteps));

        public GameLoopSteps(
            IAnimationGameLoopStep animationStep,
            IAudioGameLoopStep audioStep,
            IBehaviorSystem behaviorSystem,
            IInputSystem inputSystem,
            IPhysicsSystem physicsSystem,
            IRenderingSystem renderingSystem,
            IEnumerable<ICustomSystem> customSystems,
            CoreConfiguration configuration)
        {
            AnimationStep = animationStep;
            AudioStep = audioStep;
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
                AnimationStepName,
                AudioStepName,
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

        public IAnimationGameLoopStep AnimationStep { get; }
        public IAudioGameLoopStep AudioStep { get; }
        public IBehaviorSystem BehaviorSystem { get; }
        public IInputSystem InputSystem { get; }
        public IPhysicsSystem PhysicsSystem { get; }
        public IRenderingSystem RenderingSystem { get; }
        public IReadOnlyCollection<ICustomSystem> CustomSystems { get; }

        public string AnimationStepName => nameof(AnimationStep);
        public string AudioStepName => nameof(AudioStep);
        public string BehaviorSystemName => nameof(BehaviorSystem);
        public string InputSystemName => nameof(InputSystem);
        public string PhysicsSystemName => nameof(PhysicsSystem);
        public string RenderingSystemName => nameof(RenderingSystem);
        public IReadOnlyCollection<string> SystemsNames { get; }
    }
}