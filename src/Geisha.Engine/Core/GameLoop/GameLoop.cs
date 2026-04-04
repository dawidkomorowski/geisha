using System;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.GameLoop;

internal interface IGameLoop
{
    void Update();
}

internal sealed class GameLoop : IGameLoop
{
    private readonly ITimeSystemInternal _timeSystem;
    private readonly ICoreDiagnosticInfoProvider _coreDiagnosticInfoProvider;
    private readonly IGameLoopSteps _gameLoopSteps;
    private readonly ISceneManagerInternal _sceneManager;
    private readonly IPerformanceStatisticsRecorder _performanceStatisticsRecorder;
    private readonly int _fixedUpdatesPerFrameLimit;

    private TimeSpan _timeToSimulate;

    public GameLoop(
        ITimeSystemInternal timeSystem,
        ICoreDiagnosticInfoProvider coreDiagnosticInfoProvider,
        IGameLoopSteps gameLoopSteps,
        ISceneManagerInternal sceneManager,
        IPerformanceStatisticsRecorder performanceStatisticsRecorder,
        CoreConfiguration configuration)
    {
        _timeSystem = timeSystem;
        _coreDiagnosticInfoProvider = coreDiagnosticInfoProvider;
        _gameLoopSteps = gameLoopSteps;
        _sceneManager = sceneManager;
        _performanceStatisticsRecorder = performanceStatisticsRecorder;

        _fixedUpdatesPerFrameLimit = configuration.FixedUpdatesPerFrameLimit;
    }

    public void Update()
    {
        _sceneManager.OnNextFrame();

        var scene = _sceneManager.CurrentScene;
        var timeStep = _timeSystem.NextTimeStep();
        var fixedDeltaTime = _timeSystem.FixedDeltaTime;
        var gameTime = new GameTime(timeStep.DeltaTime);

        _timeToSimulate += timeStep.DeltaTime;
        var fixedUpdatesPerFrame = 0;

        _performanceStatisticsRecorder.BeginStepDuration();
        _gameLoopSteps.InputStep.ProcessInput();
        _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.InputStepName);

        while (_timeToSimulate >= fixedDeltaTime && (fixedUpdatesPerFrame < _fixedUpdatesPerFrameLimit || _fixedUpdatesPerFrameLimit == 0))
        {
            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.BehaviorStep.ProcessBehaviorFixedUpdate();
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.BehaviorStepName);

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.CoroutineStep.ProcessCoroutines();
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.CoroutineStepName);

            foreach (var customStep in _gameLoopSteps.CustomSteps)
            {
                _performanceStatisticsRecorder.BeginStepDuration();
                customStep.ProcessFixedUpdate();
                _performanceStatisticsRecorder.EndStepDuration(customStep.Name);
            }

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.PhysicsStep.ProcessPhysics();
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.PhysicsStepName);

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.TransformInterpolationStep.SnapshotTransforms();
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.TransformInterpolationStepName);

            scene.RemoveEntitiesAfterFixedTimeStep();

            _timeToSimulate -= fixedDeltaTime;
            fixedUpdatesPerFrame++;
        }

        _performanceStatisticsRecorder.BeginStepDuration();
        var alpha = System.Math.Min(_timeToSimulate.TotalSeconds / fixedDeltaTime.TotalSeconds, 1d);
        _gameLoopSteps.TransformInterpolationStep.InterpolateTransforms(alpha);
        _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.TransformInterpolationStepName);

        _performanceStatisticsRecorder.BeginStepDuration();
        _gameLoopSteps.BehaviorStep.ProcessBehaviorUpdate(timeStep);
        _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.BehaviorStepName);

        _performanceStatisticsRecorder.BeginStepDuration();
        _gameLoopSteps.CoroutineStep.ProcessCoroutines(timeStep);
        _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.CoroutineStepName);

        foreach (var customStep in _gameLoopSteps.CustomSteps)
        {
            _performanceStatisticsRecorder.BeginStepDuration();
            customStep.ProcessUpdate(gameTime);
            _performanceStatisticsRecorder.EndStepDuration(customStep.Name);
        }

        _performanceStatisticsRecorder.BeginStepDuration();
        _gameLoopSteps.PhysicsStep.PreparePhysicsDebugInformation();
        _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.PhysicsStepName);

        _performanceStatisticsRecorder.BeginStepDuration();
        _gameLoopSteps.AudioStep.ProcessAudio();
        _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.AudioStepName);

        _performanceStatisticsRecorder.BeginStepDuration();
        _gameLoopSteps.AnimationStep.ProcessAnimations(gameTime);
        _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.AnimationStepName);

        _performanceStatisticsRecorder.BeginStepDuration();
        _gameLoopSteps.RenderingStep.RenderScene();
        _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.RenderingStepName);

        scene.RemoveEntitiesAfterFullFrame();

        _performanceStatisticsRecorder.RecordFrame();
        _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
    }
}