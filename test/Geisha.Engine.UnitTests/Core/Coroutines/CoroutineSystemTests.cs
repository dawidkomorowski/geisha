using System;
using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Coroutines;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Coroutines
{
    [TestFixture]
    public class CoroutineSystemTests
    {
        private readonly GameTime _deltaTime = new(TimeSpan.FromMilliseconds(16));
        private CoroutineSystem _coroutineSystem = null!;
        private Scene _scene = null!;

        [SetUp]
        public void SetUp()
        {
            _coroutineSystem = new CoroutineSystem();
            _scene = TestSceneFactory.Create();
            _scene.AddObserver(_coroutineSystem);
        }

        #region CreateCoroutine

        [Test]
        public void CreateCoroutine_ShouldCreateCoroutineInPendingStateAndWithNoOwner()
        {
            // Arrange
            // Act
            var coroutine = _coroutineSystem.CreateCoroutine(WaitForNextFrameCoroutine(new Data()));

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));
            Assert.That(coroutine.OwnerEntity, Is.Null);
            Assert.That(coroutine.OwnerComponent, Is.Null);
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void CreateCoroutine_ShouldCreateCoroutineInPendingStateAndWithEntityOwner()
        {
            // Arrange
            var entity = _scene.CreateEntity();

            // Act
            var coroutine = _coroutineSystem.CreateCoroutine(WaitForNextFrameCoroutine(new Data()), entity);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));
            Assert.That(coroutine.OwnerEntity, Is.EqualTo(entity));
            Assert.That(coroutine.OwnerComponent, Is.Null);
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void CreateCoroutine_ShouldCreateCoroutineInPendingStateAndWithEntityOwnerAndComponentOwner()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var component = entity.CreateComponent<Transform2DComponent>();

            // Act
            var coroutine = _coroutineSystem.CreateCoroutine(WaitForNextFrameCoroutine(new Data()), component);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));
            Assert.That(coroutine.OwnerEntity, Is.EqualTo(entity));
            Assert.That(coroutine.OwnerComponent, Is.EqualTo(component));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        #endregion

        #region StartCoroutine

        [Test]
        public void StartCoroutine_ShouldNotExecuteCoroutine()
        {
            // Arrange
            var data = new Data();

            // Act
            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));

            // Assert
            Assert.That(data.Number, Is.Zero);
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void StartCoroutine_ShouldReturnCoroutineInRunningStateAndWithNoOwner()
        {
            // Arrange
            // Act
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()));

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
            Assert.That(coroutine.OwnerEntity, Is.Null);
            Assert.That(coroutine.OwnerComponent, Is.Null);
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void StartCoroutine_ShouldReturnCoroutineInRunningStateAndWithEntityOwner()
        {
            // Arrange
            var entity = _scene.CreateEntity();

            // Act
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), entity);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
            Assert.That(coroutine.OwnerEntity, Is.EqualTo(entity));
            Assert.That(coroutine.OwnerComponent, Is.Null);
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void StartCoroutine_ShouldReturnCoroutineInRunningStateAndWithEntityOwnerAndComponentOwner()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var component = entity.CreateComponent<Transform2DComponent>();

            // Act
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), component);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
            Assert.That(coroutine.OwnerEntity, Is.EqualTo(entity));
            Assert.That(coroutine.OwnerComponent, Is.EqualTo(component));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        #endregion

        #region ProcessCoroutines

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutineToFirstYield_WhenCalledOnce()
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));

            // Act
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(1));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutineToSecondYield_WhenCalledTwice()
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));

            // Act
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(2));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutineTillCompletion_WhenExecutedManyTimes()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));

            // Act
            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assert
            Assert.That(data.Number, Is.EqualTo(3));
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteMultipleCoroutines()
        {
            // Arrange
            var data1 = new Data();
            var data2 = new Data();
            var data3 = new Data();

            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data1));
            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data2));
            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data3));

            // Act
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data1.Number, Is.EqualTo(1));
            Assert.That(data2.Number, Is.EqualTo(1));
            Assert.That(data3.Number, Is.EqualTo(1));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(3));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteMultipleCoroutines_WhenCoroutinesStartedBetweenProcessing()
        {
            // Arrange
            var data1 = new Data();
            var data2 = new Data();
            var data3 = new Data();

            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data1));
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data2));
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data3));

            // Act
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data1.Number, Is.EqualTo(3));
            Assert.That(data2.Number, Is.EqualTo(2));
            Assert.That(data3.Number, Is.EqualTo(1));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(3));
        }

        [TestCase(1, 40, 1)]
        [TestCase(2, 40, 1)]
        [TestCase(3, 40, 1)]
        [TestCase(4, 40, 2)]
        [TestCase(12, 40, 4)]
        [TestCase(3, 100, 3)]
        [TestCase(3, 200, 3)]
        public void ProcessCoroutines_ShouldProgressCoroutineCorrectly_GivenCoroutineWithWaitTime100MS(int executionTimes, int deltaTime,
            int expectedProgressCount)
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(WaitTimeCoroutine(data));

            // Act
            for (var i = 0; i < executionTimes; i++)
            {
                _coroutineSystem.ProcessCoroutines(new GameTime(TimeSpan.FromMilliseconds(deltaTime)));
            }

            // Assert
            Assert.That(data.Number, Is.EqualTo(expectedProgressCount));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void ProcessCoroutines_ShouldProgressCoroutineOnce_WhenExecutedTwiceButConditionNotMet_GivenCoroutineWithWaitUntil()
        {
            // Arrange
            var data = new Data();

            _coroutineSystem.StartCoroutine(WaitUntilCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Act
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(1));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void ProcessCoroutines_ShouldProgressCoroutineTwice_WhenExecutedTwiceAndConditionMet_GivenCoroutineWithWaitUntil()
        {
            // Arrange
            var data = new Data();

            _coroutineSystem.StartCoroutine(WaitUntilCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            data.Condition = true;

            // Act
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(2));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void ProcessCoroutines_ShouldProgressCoroutineTwice_WhenExecuted4TimesAndConditionMetOnce_GivenCoroutineWithWaitUntil()
        {
            // Arrange
            var data = new Data();

            _coroutineSystem.StartCoroutine(WaitUntilCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);


            // Act
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            data.Condition = true;
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            data.Condition = false;
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(2));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutines_WhenExecutedManyTimes_GivenCoroutineWithCall()
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(CallCoroutine(data));

            // Act
            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assert
            Assert.That(data.Log, Is.EqualTo(new[]
            {
                "CallCoroutine - 1",
                "CallCoroutine - 2",
                "Call2Coroutine - 1",
                "Call2Coroutine - 2",
                "Call3Coroutine - 1",
                "Call3Coroutine - 2",
                "Call2Coroutine - 3",
                "CallCoroutine - 3",
                "Call3Coroutine - 1",
                "Call3Coroutine - 2",
                "CallCoroutine - 4"
            }));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void ProcessCoroutines_ShouldNotExecuteCoroutines_WhenCoroutineIsPaused_GivenCoroutineWithCall()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(CallCoroutine(data));

            // Act
            for (var i = 0; i < 5; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            coroutine.Pause();

            for (var i = 0; i < 5; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assert
            Assert.That(data.Log, Is.EqualTo(new[]
            {
                "CallCoroutine - 1",
                "CallCoroutine - 2",
                "Call2Coroutine - 1",
                "Call2Coroutine - 2",
                "Call3Coroutine - 1",
            }));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutines_WhenCoroutineIsPausedAndThenResumed_GivenCoroutineWithCall()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(CallCoroutine(data));

            // Act
            for (var i = 0; i < 5; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            coroutine.Pause();

            for (var i = 0; i < 5; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            coroutine.Resume();

            for (var i = 0; i < 5; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assert
            Assert.That(data.Log, Is.EqualTo(new[]
            {
                "CallCoroutine - 1",
                "CallCoroutine - 2",
                "Call2Coroutine - 1",
                "Call2Coroutine - 2",
                "Call3Coroutine - 1",
                "Call3Coroutine - 2",
                "Call2Coroutine - 3",
                "CallCoroutine - 3",
                "Call3Coroutine - 1",
                "Call3Coroutine - 2",
                "CallCoroutine - 4"
            }));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutines_WhenExecutedManyTimes_GivenCoroutineWithSwitchTo()
        {
            // Arrange
            var data = new Data();
            var coroutine1 = _coroutineSystem.StartCoroutine(SwitchToCoroutine1(data));
            var coroutine2 = _coroutineSystem.CreateCoroutine(SwitchToCoroutine2(data));
            data.SwitchToFrom1 = coroutine2;
            data.SwitchToFrom2 = coroutine1;

            // Act
            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assert
            Assert.That(data.Log, Is.EqualTo(new[]
            {
                "SwitchToCoroutine1 - 1",
                "SwitchToCoroutine2 - 1",
                "SwitchToCoroutine1 - 2",
                "SwitchToCoroutine2 - 2",
                "SwitchToCoroutine1 - 3",
                "SwitchToCoroutine2 - 3",
                "SwitchToCoroutine1 - 4",
                "SwitchToCoroutine2 - 4",
                "SwitchToCoroutine1 - 5",
                "SwitchToCoroutine2 - 5",
            }));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void ProcessCoroutines_ShouldThrowException_WhenCoroutineSwitchesToAlreadyActiveCoroutine()
        {
            // Arrange
            var data = new Data();
            var coroutine1 = _coroutineSystem.StartCoroutine(AnotherCoroutine(data));

            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            _coroutineSystem.StartCoroutine(SwitchToCoroutine(data, coroutine1));

            // Assume
            Assume.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));

            // Act
            // Assert
            Assert.That(() => _coroutineSystem.ProcessCoroutines(_deltaTime), Throws.InvalidOperationException);
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutines_GivenCoroutineThatStartsAnotherCoroutine()
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(StartAnotherCoroutine(data));

            // Act
            for (var i = 0; i < 5; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assert
            Assert.That(data.Log, Is.EqualTo(new[]
            {
                "StartAnotherCoroutine - 1",
                "StartAnotherCoroutine - 2",
                "StartAnotherCoroutine - 3",
                "AnotherCoroutine - 1",
                "StartAnotherCoroutine - 4",
                "AnotherCoroutine - 2",
                "AnotherCoroutine - 3"
            }));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(2));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutines_GivenCoroutineThatAbortsAnotherCoroutine()
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(AbortAnotherCoroutine(data));
            var coroutineToAbort = _coroutineSystem.StartCoroutine(AnotherCoroutine(data));
            data.CoroutineToAbort = coroutineToAbort;

            // Act
            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assert
            Assert.That(data.Log, Is.EqualTo(new[]
            {
                "AbortAnotherCoroutine - 1",
                "AnotherCoroutine - 1",
                "AbortAnotherCoroutine - 2",
                "AbortAnotherCoroutine - 3"
            }));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        #endregion

        #region Coroutine.Abort

        [Test]
        public void Coroutine_Abort_ShouldAbortCoroutineFromBeingExecuted()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Act
            coroutine.Abort();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(1));
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void Coroutine_Abort_ShouldThrowException_WhenCoroutineIsInCompletedState()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

            // Act
            // Assert
            Assert.That(() => coroutine.Abort(), Throws.InvalidOperationException);
        }

        #endregion

        #region Coroutine.Pause

        [Test]
        public void Coroutine_Pause_ShouldPauseCoroutineFromBeingExecuted()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Act
            coroutine.Pause();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(1));
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Paused));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void Coroutine_Pause_ShouldThrowException_WhenCoroutineIsInPendingState()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.CreateCoroutine(WaitForNextFrameCoroutine(data));

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));

            // Act
            // Assert
            Assert.That(() => coroutine.Pause(), Throws.InvalidOperationException);
        }

        [Test]
        public void Coroutine_Pause_ShouldThrowException_WhenCoroutineIsInCompletedState()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

            // Act
            // Assert
            Assert.That(() => coroutine.Pause(), Throws.InvalidOperationException);
        }

        [Test]
        public void Coroutine_Pause_ShouldThrowException_WhenCoroutineIsInAbortedState()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));
            coroutine.Abort();

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));

            // Act
            // Assert
            Assert.That(() => coroutine.Pause(), Throws.InvalidOperationException);
        }

        #endregion

        #region Coroutine.Resume

        [Test]
        public void Coroutine_Resume_ShouldResumePausedCoroutineSoItContinuesExecuting()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            coroutine.Pause();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Act
            coroutine.Resume();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(2));
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
        }

        [Test]
        public void Coroutine_Resume_ShouldThrowException_WhenCoroutineIsInPendingState()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.CreateCoroutine(WaitForNextFrameCoroutine(data));

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));

            // Act
            // Assert
            Assert.That(() => coroutine.Resume(), Throws.InvalidOperationException);
        }

        [Test]
        public void Coroutine_Resume_ShouldThrowException_WhenCoroutineIsInCompletedState()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

            // Act
            // Assert
            Assert.That(() => coroutine.Resume(), Throws.InvalidOperationException);
        }

        [Test]
        public void Coroutine_Resume_ShouldThrowException_WhenCoroutineIsInAbortedState()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));
            coroutine.Abort();

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));

            // Act
            // Assert
            Assert.That(() => coroutine.Resume(), Throws.InvalidOperationException);
        }

        #endregion

        #region RemoveEntity

        [Test]
        public void RemoveEntity_ShouldAbortCoroutineOwnedByEntity()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), entity);

            // Act
            entity.RemoveAfterFullFrame();
            _scene.RemoveEntitiesAfterFullFrame();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveEntity_ShouldAbortAllCoroutinesOwnedByEntity()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var coroutine1 = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), entity);
            var coroutine2 = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), entity);
            var coroutine3 = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), entity);

            // Act
            entity.RemoveAfterFullFrame();
            _scene.RemoveEntitiesAfterFullFrame();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine1.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(coroutine2.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(coroutine3.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveEntity_ShouldAbortCoroutineOwnedByEntity_WhenOtherCoroutineOwnedByEntityWasCompleted()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var coroutine1 = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), entity);
            var coroutine2 = _coroutineSystem.StartCoroutine(AnotherCoroutine(new Data()), entity);

            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assume
            Assume.That(coroutine1.State, Is.EqualTo(CoroutineState.Completed));

            // Act
            entity.RemoveAfterFullFrame();
            _scene.RemoveEntitiesAfterFullFrame();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine2.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveEntity_ShouldAbortCoroutineOwnedByComponentOfThatEntity()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var component = entity.CreateComponent<Transform2DComponent>();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), component);

            // Act
            entity.RemoveAfterFullFrame();
            _scene.RemoveEntitiesAfterFullFrame();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveEntity_ShouldNotChangeStateOfCompletedCoroutineOwnedByEntity()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), entity);

            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

            // Act
            entity.RemoveAfterFullFrame();
            _scene.RemoveEntitiesAfterFullFrame();
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveEntity_ShouldNotChangeStateOfCompletedCoroutineOwnedByEntity_WhenEntityRemovedBeforeNextProcessCoroutines()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var coroutine1 = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), entity);
            var coroutine2 = _coroutineSystem.StartCoroutine(RemoveEntityCoroutine(entity));

            // Act
            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
                _scene.RemoveEntitiesAfterFullFrame();
            }

            // Assert
            Assert.That(coroutine1.State, Is.EqualTo(CoroutineState.Completed));
            Assert.That(coroutine2.State, Is.EqualTo(CoroutineState.Completed));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        #endregion

        #region RemoveComponent

        [Test]
        public void RemoveComponent_ShouldAbortCoroutineOwnedByComponent()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var component = entity.CreateComponent<Transform2DComponent>();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), component);

            // Act
            entity.RemoveComponent(component);
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveComponent_ShouldAbortAllCoroutinesOwnedByComponent()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var component = entity.CreateComponent<Transform2DComponent>();
            var coroutine1 = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), component);
            var coroutine2 = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), component);
            var coroutine3 = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), component);

            // Act
            entity.RemoveComponent(component);
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine1.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(coroutine2.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(coroutine3.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveComponent_ShouldAbortCoroutineOwnedByComponent_WhenOtherCoroutineOwnedByComponentWasCompleted()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var component = entity.CreateComponent<Transform2DComponent>();
            var coroutine1 = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), component);
            var coroutine2 = _coroutineSystem.StartCoroutine(AnotherCoroutine(new Data()), component);

            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assume
            Assume.That(coroutine1.State, Is.EqualTo(CoroutineState.Completed));

            // Act
            entity.RemoveComponent(component);
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine2.State, Is.EqualTo(CoroutineState.Aborted));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveComponent_ShouldNotChangeStateOfCompletedCoroutineOwnedByComponent()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var component = entity.CreateComponent<Transform2DComponent>();
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()), component);

            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assume
            Assume.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

            // Act
            entity.RemoveComponent(component);
            _coroutineSystem.ProcessCoroutines(_deltaTime);

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        [Test]
        public void RemoveComponent_ShouldNotChangeStateOfCompletedCoroutineOwnedByComponent_WhenComponentRemovedInAnotherCoroutine()
        {
            // Arrange
            var entity = _scene.CreateEntity();
            var component = entity.CreateComponent<Transform2DComponent>();
            var coroutine1 = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), component);
            var coroutine2 = _coroutineSystem.StartCoroutine(RemoveComponentCoroutine(component));

            // Act
            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
            }

            // Assert
            Assert.That(coroutine1.State, Is.EqualTo(CoroutineState.Completed));
            Assert.That(coroutine2.State, Is.EqualTo(CoroutineState.Completed));
            Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
        }

        #endregion

        #region Helpers

        private static IEnumerator<CoroutineInstruction> WaitForNextFrameCoroutine(Data data)
        {
            data.Number++;
            yield return Coroutine.WaitForNextFrame();

            data.Number++;
            yield return Coroutine.WaitForNextFrame();

            data.Number++;
            yield return Coroutine.WaitForNextFrame();
        }

        private static IEnumerator<CoroutineInstruction> WaitTimeCoroutine(Data data)
        {
            while (true)
            {
                data.Number++;
                yield return Coroutine.Wait(TimeSpan.FromMilliseconds(100));
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static IEnumerator<CoroutineInstruction> WaitUntilCoroutine(Data data)
        {
            while (true)
            {
                data.Number++;
                yield return Coroutine.WaitUntil(() => data.Condition);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static IEnumerator<CoroutineInstruction> CallCoroutine(Data data)
        {
            data.Log.Add($"{nameof(CallCoroutine)} - 1");
            yield return Coroutine.WaitForNextFrame();
            data.Log.Add($"{nameof(CallCoroutine)} - 2");
            yield return Coroutine.Call(Call2Coroutine(data));
            data.Log.Add($"{nameof(CallCoroutine)} - 3");
            yield return Coroutine.Call(Call3Coroutine(data));
            data.Log.Add($"{nameof(CallCoroutine)} - 4");
        }

        private static IEnumerator<CoroutineInstruction> Call2Coroutine(Data data)
        {
            data.Log.Add($"{nameof(Call2Coroutine)} - 1");
            yield return Coroutine.WaitForNextFrame();
            data.Log.Add($"{nameof(Call2Coroutine)} - 2");
            yield return Coroutine.Call(Call3Coroutine(data));
            data.Log.Add($"{nameof(Call2Coroutine)} - 3");
        }

        private static IEnumerator<CoroutineInstruction> Call3Coroutine(Data data)
        {
            data.Log.Add($"{nameof(Call3Coroutine)} - 1");
            yield return Coroutine.WaitForNextFrame();
            data.Log.Add($"{nameof(Call3Coroutine)} - 2");
        }

        private static IEnumerator<CoroutineInstruction> SwitchToCoroutine(Data data, Coroutine switchTo)
        {
            data.Log.Add($"{nameof(SwitchToCoroutine)}");
            yield return Coroutine.SwitchTo(switchTo);
        }

        private static IEnumerator<CoroutineInstruction> SwitchToCoroutine1(Data data)
        {
            var i = 0;
            while (true)
            {
                i++;
                data.Log.Add($"{nameof(SwitchToCoroutine1)} - {i}");
                Debug.Assert(data.SwitchToFrom1 != null, "data.SwitchToFrom1 != null");
                yield return Coroutine.SwitchTo(data.SwitchToFrom1);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static IEnumerator<CoroutineInstruction> SwitchToCoroutine2(Data data)
        {
            var i = 0;
            while (true)
            {
                i++;
                data.Log.Add($"{nameof(SwitchToCoroutine2)} - {i}");
                Debug.Assert(data.SwitchToFrom2 != null, "data.SwitchToFrom2 != null");
                yield return Coroutine.SwitchTo(data.SwitchToFrom2);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator<CoroutineInstruction> StartAnotherCoroutine(Data data)
        {
            data.Log.Add($"{nameof(StartAnotherCoroutine)} - 1");
            yield return Coroutine.WaitForNextFrame();

            _coroutineSystem.StartCoroutine(AnotherCoroutine(data));
            data.Log.Add($"{nameof(StartAnotherCoroutine)} - 2");
            yield return Coroutine.WaitForNextFrame();

            data.Log.Add($"{nameof(StartAnotherCoroutine)} - 3");
            yield return Coroutine.WaitForNextFrame();

            data.Log.Add($"{nameof(StartAnotherCoroutine)} - 4");
            yield return Coroutine.WaitForNextFrame();
        }

        private static IEnumerator<CoroutineInstruction> AnotherCoroutine(Data data)
        {
            var i = 0;
            while (true)
            {
                i++;
                data.Log.Add($"{nameof(AnotherCoroutine)} - {i}");
                yield return Coroutine.WaitForNextFrame();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static IEnumerator<CoroutineInstruction> AbortAnotherCoroutine(Data data)
        {
            data.Log.Add($"{nameof(AbortAnotherCoroutine)} - 1");
            yield return Coroutine.WaitForNextFrame();

            Debug.Assert(data.CoroutineToAbort != null, "data.CoroutineToAbort != null");
            data.CoroutineToAbort.Abort();
            data.Log.Add($"{nameof(AbortAnotherCoroutine)} - 2");
            yield return Coroutine.WaitForNextFrame();

            data.Log.Add($"{nameof(AbortAnotherCoroutine)} - 3");
            yield return Coroutine.WaitForNextFrame();
        }

        private static IEnumerator<CoroutineInstruction> CompleteAfterFirstRunCoroutine()
        {
            yield break;
        }

        private static IEnumerator<CoroutineInstruction> RemoveEntityCoroutine(Entity entity)
        {
            entity.RemoveAfterFullFrame();
            yield break;
        }

        private static IEnumerator<CoroutineInstruction> RemoveComponentCoroutine(Component component)
        {
            component.Entity.RemoveComponent(component);
            yield break;
        }

        private sealed class Data
        {
            public int Number { get; set; }
            public bool Condition { get; set; }
            public List<string> Log { get; } = new();
            public Coroutine? SwitchToFrom1 { get; set; }
            public Coroutine? SwitchToFrom2 { get; set; }
            public Coroutine? CoroutineToAbort { get; set; }
        }

        #endregion
    }
}