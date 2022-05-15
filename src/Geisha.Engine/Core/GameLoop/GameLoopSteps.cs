using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;

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
        private static readonly ILog Log = LogFactory.Create(typeof(GameLoopSteps));

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

            Log.Info("Available custom game loop steps:");
            foreach (var customStep in customStepsList)
            {
                Log.Info($"-> {customStep.Name}");
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
                    throw new ArgumentException($"Cannot find custom game loop step specified in configuration. Custom step name: {stepName}");

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

            Log.Info("Custom game loop steps has been configured to execute in following order:");
            foreach (var customStep in CustomSteps)
            {
                Log.Info($"-> {customStep.Name}");
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