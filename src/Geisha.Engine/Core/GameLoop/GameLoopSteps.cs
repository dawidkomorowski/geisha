using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Systems;
using NLog;

namespace Geisha.Engine.Core.GameLoop
{
    internal interface IGameLoopSteps
    {
        IAnimationGameLoopStep AnimationStep { get; }
        IAudioGameLoopStep AudioStep { get; }
        IBehaviorGameLoopStep BehaviorStep { get; }
        IInputGameLoopStep InputStep { get; }
        IPhysicsGameLoopStep PhysicsStep { get; }
        IRenderingGameLoopStep RenderingStep { get; }
        IReadOnlyCollection<ICustomGameLoopStep> CustomSteps { get; }

        string AnimationStepName { get; }
        string AudioStepName { get; }
        string BehaviorStepName { get; }
        string InputStepName { get; }
        string PhysicsStepName { get; }
        string RenderingStepName { get; }
        IReadOnlyCollection<string> StepsNames { get; }
    }

    internal sealed class GameLoopSteps : IGameLoopSteps
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GameLoopSteps(
            IAnimationGameLoopStep animationStep,
            IAudioGameLoopStep audioStep,
            IBehaviorGameLoopStep behaviorStep,
            IInputGameLoopStep inputStep,
            IPhysicsGameLoopStep physicsStep,
            IRenderingGameLoopStep renderingStep,
            IEnumerable<ICustomGameLoopStep> customSteps,
            CoreConfiguration configuration)
        {
            AnimationStep = animationStep;
            AudioStep = audioStep;
            BehaviorStep = behaviorStep;
            InputStep = inputStep;
            PhysicsStep = physicsStep;
            RenderingStep = renderingStep;

            var configuredCustomGameLoopSteps = configuration.CustomGameLoopSteps;
            var customStepsList = customSteps.ToList();
            var customStepsSortedList = new List<ICustomGameLoopStep>();

            Logger.Debug("Available custom game loop steps:");
            foreach (var customStep in customStepsList)
            {
                Logger.Debug("-> {0}", customStep.Name);
            }

            if (configuredCustomGameLoopSteps.Count != configuredCustomGameLoopSteps.Distinct().Count())
                throw new ArgumentException("Configuration specifies duplicated custom game loop steps. Each custom step can be specified only once.");

            var customStepsNames = customStepsList.Select(cs => cs.Name).ToList();
            if (customStepsNames.Count != customStepsNames.Distinct().Count())
                throw new ArgumentException("There are custom game loop steps with duplicated names. Each step must have unique name.");

            foreach (var stepName in configuredCustomGameLoopSteps)
            {
                var customStep = customStepsList.SingleOrDefault(cs => cs.Name == stepName);
                if (customStep == null)
                {
                    var error = $"Cannot find custom game loop step specified in configuration. Custom step name: {stepName}";
                    var suggestion1 = $"Make sure the custom game loop step is registered in {nameof(Game)}.{nameof(Game.RegisterComponents)}.";
                    var suggestion2 =
                        $"If your custom game loop step implements {nameof(ICustomSystem)} interface then register it with {nameof(IComponentsRegistry)}.{nameof(IComponentsRegistry.RegisterSystem)}.";
                    var message = $"{error}{Environment.NewLine}{Environment.NewLine}{suggestion1} {suggestion2}";

                    throw new ArgumentException(message);
                }

                customStepsSortedList.Add(customStep);
            }

            CustomSteps = customStepsSortedList.AsReadOnly();

            StepsNames = new[]
            {
                AnimationStepName,
                AudioStepName,
                BehaviorStepName,
                InputStepName,
                PhysicsStepName,
                RenderingStepName
            }.Concat(CustomSteps.Select(cs => cs.Name)).OrderBy(n => n).ToList().AsReadOnly();

            Logger.Debug("Custom game loop steps has been configured to execute in following order:");
            foreach (var customStep in CustomSteps)
            {
                Logger.Debug("-> {0}", customStep.Name);
            }
        }

        public IAnimationGameLoopStep AnimationStep { get; }
        public IAudioGameLoopStep AudioStep { get; }
        public IBehaviorGameLoopStep BehaviorStep { get; }
        public IInputGameLoopStep InputStep { get; }
        public IPhysicsGameLoopStep PhysicsStep { get; }
        public IRenderingGameLoopStep RenderingStep { get; }
        public IReadOnlyCollection<ICustomGameLoopStep> CustomSteps { get; }

        public string AnimationStepName => nameof(AnimationStep);
        public string AudioStepName => nameof(AudioStep);
        public string BehaviorStepName => nameof(BehaviorStep);
        public string InputStepName => nameof(InputStep);
        public string PhysicsStepName => nameof(PhysicsStep);
        public string RenderingStepName => nameof(RenderingStep);
        public IReadOnlyCollection<string> StepsNames { get; }
    }
}