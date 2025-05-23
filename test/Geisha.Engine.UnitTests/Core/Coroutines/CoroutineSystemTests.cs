﻿using System;
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
    public abstract class CoroutineSystemTests
    {
        private CoroutineSystem _coroutineSystem = null!;
        private Scene _scene = null!;

        [SetUp]
        public void SetUp()
        {
            _coroutineSystem = new CoroutineSystem();
            _scene = TestSceneFactory.Create();
            _scene.AddObserver(_coroutineSystem);
        }

        private static IEnumerator<CoroutineInstruction> UpdateEveryFrameCoroutine(Data data)
        {
            var i = 0;
            while (true)
            {
                i++;
                data.Log.Add($"{nameof(UpdateEveryFrameCoroutine)} - {i}");
                yield return Coroutine.WaitForNextFrame();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static IEnumerator<CoroutineInstruction> WaitForNextFrameCoroutine(Data data)
        {
            data.Log.Add($"{nameof(WaitForNextFrameCoroutine)} - 1");
            yield return Coroutine.WaitForNextFrame();

            data.Log.Add($"{nameof(WaitForNextFrameCoroutine)} - 2");
            yield return Coroutine.WaitForNextFrame();

            data.Log.Add($"{nameof(WaitForNextFrameCoroutine)} - 3");
            yield return Coroutine.WaitForNextFrame();
        }

        private static IEnumerator<CoroutineInstruction> WaitTimeCoroutine(Data data)
        {
            var i = 0;
            while (true)
            {
                i++;
                data.Log.Add($"{nameof(WaitTimeCoroutine)} - {i}");
                yield return Coroutine.Wait(TimeSpan.FromMilliseconds(100));
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static IEnumerator<CoroutineInstruction> WaitUntilCoroutine(Data data)
        {
            var i = 0;
            while (true)
            {
                i++;
                data.Log.Add($"{nameof(WaitUntilCoroutine)} - {i}");
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
            public List<string> Log { get; } = new();
            public bool Condition { get; set; }
            public Coroutine? SwitchToFrom1 { get; set; }
            public Coroutine? SwitchToFrom2 { get; set; }
            public Coroutine? CoroutineToAbort { get; set; }
        }

        #region Common tests

        [TestFixture]
        public abstract class CommonCoroutineSystemTests : CoroutineSystemTests
        {
            protected abstract CoroutineUpdateMode UpdateMode { get; }
            protected abstract void ProcessCoroutines();

            #region CreateCoroutine

            [Test]
            public void CreateCoroutine_ShouldCreateCoroutineInPendingStateAndWithCorrectUpdateModeAndWithNoOwner()
            {
                // Arrange
                // Act
                var coroutine = _coroutineSystem.CreateCoroutine(UpdateEveryFrameCoroutine(new Data()), UpdateMode);

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));
                Assert.That(coroutine.UpdateMode, Is.EqualTo(UpdateMode));
                Assert.That(coroutine.OwnerEntity, Is.Null);
                Assert.That(coroutine.OwnerComponent, Is.Null);
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void CreateCoroutine_ShouldCreateCoroutineInPendingStateAndWithCorrectUpdateModeAndWithEntityOwner()
            {
                // Arrange
                var entity = _scene.CreateEntity();

                // Act
                var coroutine = _coroutineSystem.CreateCoroutine(UpdateEveryFrameCoroutine(new Data()), entity, UpdateMode);

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));
                Assert.That(coroutine.UpdateMode, Is.EqualTo(UpdateMode));
                Assert.That(coroutine.OwnerEntity, Is.EqualTo(entity));
                Assert.That(coroutine.OwnerComponent, Is.Null);
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void CreateCoroutine_ShouldCreateCoroutineInPendingStateAndWithCorrectUpdateModeAndWithEntityOwnerAndComponentOwner()
            {
                // Arrange
                var entity = _scene.CreateEntity();
                var component = entity.CreateComponent<Transform2DComponent>();

                // Act
                var coroutine = _coroutineSystem.CreateCoroutine(UpdateEveryFrameCoroutine(new Data()), component, UpdateMode);

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));
                Assert.That(coroutine.UpdateMode, Is.EqualTo(UpdateMode));
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
                _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);

                // Assert
                Assert.That(data.Log, Is.Empty);
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void StartCoroutine_ShouldReturnCoroutineInRunningStateAndWithCorrectUpdateModeAndWithNoOwner()
            {
                // Arrange
                // Act
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), UpdateMode);

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
                Assert.That(coroutine.UpdateMode, Is.EqualTo(UpdateMode));
                Assert.That(coroutine.OwnerEntity, Is.Null);
                Assert.That(coroutine.OwnerComponent, Is.Null);
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void StartCoroutine_ShouldReturnCoroutineInRunningStateAndWithCorrectUpdateModeAndWithEntityOwner()
            {
                // Arrange
                var entity = _scene.CreateEntity();

                // Act
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), entity, UpdateMode);

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
                Assert.That(coroutine.UpdateMode, Is.EqualTo(UpdateMode));
                Assert.That(coroutine.OwnerEntity, Is.EqualTo(entity));
                Assert.That(coroutine.OwnerComponent, Is.Null);
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void StartCoroutine_ShouldReturnCoroutineInRunningStateAndWithCorrectUpdateModeAndWithEntityOwnerAndComponentOwner()
            {
                // Arrange
                var entity = _scene.CreateEntity();
                var component = entity.CreateComponent<Transform2DComponent>();

                // Act
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), component, UpdateMode);

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
                Assert.That(coroutine.UpdateMode, Is.EqualTo(UpdateMode));
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
                _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data), UpdateMode);

                // Act
                ProcessCoroutines();

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldExecuteCoroutineToSecondYield_WhenCalledTwice()
            {
                // Arrange
                var data = new Data();
                _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data), UpdateMode);

                // Act
                ProcessCoroutines();
                ProcessCoroutines();

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1",
                    "WaitForNextFrameCoroutine - 2"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldExecuteCoroutineTillCompletion_WhenExecutedManyTimes()
            {
                // Arrange
                var data = new Data();
                var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data), UpdateMode);

                // Act
                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1",
                    "WaitForNextFrameCoroutine - 2",
                    "WaitForNextFrameCoroutine - 3"
                }));
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

                _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data1), UpdateMode);
                _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data2), UpdateMode);
                _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data3), UpdateMode);

                // Act
                ProcessCoroutines();

                // Assert
                Assert.That(data1.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1"
                }));
                Assert.That(data2.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1"
                }));
                Assert.That(data3.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(3));
            }

            [Test]
            public void ProcessCoroutines_ShouldExecuteMultipleCoroutines_WhenCoroutinesStartedBetweenProcessing()
            {
                // Arrange
                var data1 = new Data();
                var data2 = new Data();
                var data3 = new Data();

                _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data1), UpdateMode);
                ProcessCoroutines();
                _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data2), UpdateMode);
                ProcessCoroutines();
                _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data3), UpdateMode);

                // Act
                ProcessCoroutines();

                // Assert
                Assert.That(data1.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1",
                    "WaitForNextFrameCoroutine - 2",
                    "WaitForNextFrameCoroutine - 3"
                }));
                Assert.That(data2.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1",
                    "WaitForNextFrameCoroutine - 2"
                }));
                Assert.That(data3.Log, Is.EqualTo(new[]
                {
                    "WaitForNextFrameCoroutine - 1"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(3));
            }

            [Test]
            public void ProcessCoroutines_ShouldProgressCoroutineOnce_WhenExecutedTwiceButConditionNotMet_GivenCoroutineWithWaitUntil()
            {
                // Arrange
                var data = new Data();

                _coroutineSystem.StartCoroutine(WaitUntilCoroutine(data), UpdateMode);
                ProcessCoroutines();

                // Act
                ProcessCoroutines();

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "WaitUntilCoroutine - 1"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldProgressCoroutineTwice_WhenExecutedTwiceAndConditionMet_GivenCoroutineWithWaitUntil()
            {
                // Arrange
                var data = new Data();

                _coroutineSystem.StartCoroutine(WaitUntilCoroutine(data), UpdateMode);
                ProcessCoroutines();

                data.Condition = true;

                // Act
                ProcessCoroutines();

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "WaitUntilCoroutine - 1",
                    "WaitUntilCoroutine - 2"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldProgressCoroutineTwice_WhenExecuted4TimesAndConditionMetOnce_GivenCoroutineWithWaitUntil()
            {
                // Arrange
                var data = new Data();

                _coroutineSystem.StartCoroutine(WaitUntilCoroutine(data), UpdateMode);
                ProcessCoroutines();


                // Act
                ProcessCoroutines();
                data.Condition = true;
                ProcessCoroutines();
                data.Condition = false;
                ProcessCoroutines();

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "WaitUntilCoroutine - 1",
                    "WaitUntilCoroutine - 2"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldExecuteCoroutines_WhenExecutedManyTimes_GivenCoroutineWithCall()
            {
                // Arrange
                var data = new Data();
                _coroutineSystem.StartCoroutine(CallCoroutine(data), UpdateMode);

                // Act
                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
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
                var coroutine = _coroutineSystem.StartCoroutine(CallCoroutine(data), UpdateMode);

                // Act
                for (var i = 0; i < 5; i++)
                {
                    ProcessCoroutines();
                }

                coroutine.Pause();

                for (var i = 0; i < 5; i++)
                {
                    ProcessCoroutines();
                }

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "CallCoroutine - 1",
                    "CallCoroutine - 2",
                    "Call2Coroutine - 1",
                    "Call2Coroutine - 2",
                    "Call3Coroutine - 1"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldExecuteCoroutines_WhenCoroutineIsPausedAndThenResumed_GivenCoroutineWithCall()
            {
                // Arrange
                var data = new Data();
                var coroutine = _coroutineSystem.StartCoroutine(CallCoroutine(data), UpdateMode);

                // Act
                for (var i = 0; i < 5; i++)
                {
                    ProcessCoroutines();
                }

                coroutine.Pause();

                for (var i = 0; i < 5; i++)
                {
                    ProcessCoroutines();
                }

                coroutine.Resume();

                for (var i = 0; i < 5; i++)
                {
                    ProcessCoroutines();
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
                var coroutine1 = _coroutineSystem.StartCoroutine(SwitchToCoroutine1(data), UpdateMode);
                var coroutine2 = _coroutineSystem.CreateCoroutine(SwitchToCoroutine2(data), UpdateMode);
                data.SwitchToFrom1 = coroutine2;
                data.SwitchToFrom2 = coroutine1;

                // Act
                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
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
                    "SwitchToCoroutine2 - 5"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldNotExecuteCoroutine_WhenCoroutineSwitchesToPausedCoroutine()
            {
                // Arrange
                var data = new Data();
                var coroutine1 = _coroutineSystem.StartCoroutine(SwitchToCoroutine1(data), UpdateMode);
                var coroutine2 = _coroutineSystem.CreateCoroutine(SwitchToCoroutine2(data), UpdateMode);
                data.SwitchToFrom1 = coroutine2;
                data.SwitchToFrom2 = coroutine1;

                // Act
                for (var i = 0; i < 4; i++)
                {
                    ProcessCoroutines();
                }

                coroutine2.Pause();

                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "SwitchToCoroutine1 - 1",
                    "SwitchToCoroutine2 - 1",
                    "SwitchToCoroutine1 - 2",
                    "SwitchToCoroutine2 - 2",
                    "SwitchToCoroutine1 - 3"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldNotExecuteCoroutine_WhenCoroutineSwitchesToPausedCoroutineWhichIsThenResumed()
            {
                // Arrange
                var data = new Data();
                var coroutine1 = _coroutineSystem.StartCoroutine(SwitchToCoroutine1(data), UpdateMode);
                var coroutine2 = _coroutineSystem.CreateCoroutine(SwitchToCoroutine2(data), UpdateMode);
                data.SwitchToFrom1 = coroutine2;
                data.SwitchToFrom2 = coroutine1;

                // Act
                for (var i = 0; i < 4; i++)
                {
                    ProcessCoroutines();
                }

                coroutine2.Pause();

                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                coroutine2.Resume();

                for (var i = 0; i < 5; i++)
                {
                    ProcessCoroutines();
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
                    "SwitchToCoroutine2 - 5"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void ProcessCoroutines_ShouldThrowException_WhenCoroutineSwitchesToActiveCoroutine()
            {
                // Arrange
                var data = new Data();
                var coroutine1 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);

                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                _coroutineSystem.StartCoroutine(SwitchToCoroutine(data, coroutine1), UpdateMode);

                // Assume
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));

                // Act
                // Assert
                Assert.That(ProcessCoroutines, Throws.InvalidOperationException.With.Message.EqualTo("Cannot switch to coroutine that is already active."));
            }

            [Test]
            public void ProcessCoroutines_ShouldThrowException_WhenCoroutineSwitchesToAbortedCoroutine()
            {
                // Arrange
                var data = new Data();
                var coroutine1 = _coroutineSystem.StartCoroutine(SwitchToCoroutine1(data), UpdateMode);
                var coroutine2 = _coroutineSystem.CreateCoroutine(SwitchToCoroutine2(data), UpdateMode);
                data.SwitchToFrom1 = coroutine2;
                data.SwitchToFrom2 = coroutine1;

                ProcessCoroutines();
                ProcessCoroutines();

                coroutine2.Abort();

                // Act
                // Assert
                Assert.That(ProcessCoroutines, Throws.InvalidOperationException.With.Message.EqualTo("Cannot switch to aborted coroutine."));
            }

            [Test]
            public void ProcessCoroutines_ShouldThrowException_WhenCoroutineSwitchesToCompletedCoroutine()
            {
                // Arrange
                var data1 = new Data();
                var coroutine1 = _coroutineSystem.StartCoroutine(SwitchToCoroutine1(data1), UpdateMode);
                var coroutine2 = _coroutineSystem.CreateCoroutine(SwitchToCoroutine2(data1), UpdateMode);
                data1.SwitchToFrom1 = coroutine2;
                data1.SwitchToFrom2 = coroutine1;

                var data2 = new Data();
                var coroutine3 = _coroutineSystem.StartCoroutine(SwitchToCoroutine1(data2), UpdateMode);
                var completedCoroutine = _coroutineSystem.CreateCoroutine(SwitchToCoroutine(data2, coroutine3), UpdateMode);
                data2.SwitchToFrom1 = completedCoroutine;

                for (var i = 0; i < 5; i++)
                {
                    ProcessCoroutines();
                }

                data1.SwitchToFrom1 = completedCoroutine;
                data1.SwitchToFrom2 = completedCoroutine;

                // Assume
                Assert.That(completedCoroutine.State, Is.EqualTo(CoroutineState.Completed));

                // Act
                // Assert
                Assert.That(ProcessCoroutines, Throws.InvalidOperationException.With.Message.EqualTo("Cannot switch to completed coroutine."));
            }

            [Test]
            public void ProcessCoroutines_ShouldExecuteCoroutines_GivenCoroutineThatStartsAnotherCoroutine()
            {
                // Arrange
                var data = new Data();
                _coroutineSystem.StartCoroutine(StartAnotherCoroutine(data), UpdateMode);

                // Act
                for (var i = 0; i < 5; i++)
                {
                    ProcessCoroutines();
                }

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "StartAnotherCoroutine - 1",
                    "StartAnotherCoroutine - 2",
                    "StartAnotherCoroutine - 3",
                    "UpdateEveryFrameCoroutine - 1",
                    "StartAnotherCoroutine - 4",
                    "UpdateEveryFrameCoroutine - 2",
                    "UpdateEveryFrameCoroutine - 3"
                }));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(2));
            }

            [Test]
            public void ProcessCoroutines_ShouldExecuteCoroutines_GivenCoroutineThatAbortsAnotherCoroutine()
            {
                // Arrange
                var data = new Data();
                _coroutineSystem.StartCoroutine(AbortAnotherCoroutine(data), UpdateMode);
                var coroutineToAbort = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);
                data.CoroutineToAbort = coroutineToAbort;

                // Act
                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "AbortAnotherCoroutine - 1",
                    "UpdateEveryFrameCoroutine - 1",
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
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);
                ProcessCoroutines();

                // Act
                coroutine.Abort();
                ProcessCoroutines();

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "UpdateEveryFrameCoroutine - 1"
                }));
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void Coroutine_Abort_ShouldThrowException_WhenCoroutineIsInCompletedState()
            {
                // Arrange
                var coroutine = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), UpdateMode);
                ProcessCoroutines();

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

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
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);
                ProcessCoroutines();

                // Act
                coroutine.Pause();
                ProcessCoroutines();

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "UpdateEveryFrameCoroutine - 1"
                }));
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Paused));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void Coroutine_Pause_ShouldThrowException_WhenCoroutineIsInPendingState()
            {
                // Arrange
                var data = new Data();
                var coroutine = _coroutineSystem.CreateCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));

                // Act
                // Assert
                Assert.That(() => coroutine.Pause(), Throws.InvalidOperationException);
            }

            [Test]
            public void Coroutine_Pause_ShouldThrowException_WhenCoroutineIsInCompletedState()
            {
                // Arrange
                var coroutine = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), UpdateMode);
                ProcessCoroutines();

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

                // Act
                // Assert
                Assert.That(() => coroutine.Pause(), Throws.InvalidOperationException);
            }

            [Test]
            public void Coroutine_Pause_ShouldThrowException_WhenCoroutineIsInAbortedState()
            {
                // Arrange
                var data = new Data();
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);
                coroutine.Abort();

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));

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
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);
                ProcessCoroutines();
                coroutine.Pause();
                ProcessCoroutines();

                // Act
                coroutine.Resume();
                ProcessCoroutines();

                // Assert
                Assert.That(data.Log, Is.EqualTo(new[]
                {
                    "UpdateEveryFrameCoroutine - 1",
                    "UpdateEveryFrameCoroutine - 2"
                }));
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }

            [Test]
            public void Coroutine_Resume_ShouldThrowException_WhenCoroutineIsInPendingState()
            {
                // Arrange
                var data = new Data();
                var coroutine = _coroutineSystem.CreateCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));

                // Act
                // Assert
                Assert.That(() => coroutine.Resume(), Throws.InvalidOperationException);
            }

            [Test]
            public void Coroutine_Resume_ShouldThrowException_WhenCoroutineIsInCompletedState()
            {
                // Arrange
                var coroutine = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), UpdateMode);
                ProcessCoroutines();

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

                // Act
                // Assert
                Assert.That(() => coroutine.Resume(), Throws.InvalidOperationException);
            }

            [Test]
            public void Coroutine_Resume_ShouldThrowException_WhenCoroutineIsInAbortedState()
            {
                // Arrange
                var data = new Data();
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);
                coroutine.Abort();

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));

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
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), entity, UpdateMode);

                // Act
                entity.RemoveAfterFullFrame();
                _scene.RemoveEntitiesAfterFullFrame();
                ProcessCoroutines();

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void RemoveEntity_ShouldAbortAllCoroutinesOwnedByEntity()
            {
                // Arrange
                var entity = _scene.CreateEntity();
                var coroutine1 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), entity, UpdateMode);
                var coroutine2 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), entity, UpdateMode);
                var coroutine3 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), entity, UpdateMode);

                // Act
                entity.RemoveAfterFullFrame();
                _scene.RemoveEntitiesAfterFullFrame();
                ProcessCoroutines();

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
                var coroutine1 = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), entity, UpdateMode);
                var coroutine2 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), entity, UpdateMode);

                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                // Assume
                Assert.That(coroutine1.State, Is.EqualTo(CoroutineState.Completed));

                // Act
                entity.RemoveAfterFullFrame();
                _scene.RemoveEntitiesAfterFullFrame();
                ProcessCoroutines();

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
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), component, UpdateMode);

                // Act
                entity.RemoveAfterFullFrame();
                _scene.RemoveEntitiesAfterFullFrame();
                ProcessCoroutines();

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Aborted));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void RemoveEntity_ShouldNotChangeStateOfCompletedCoroutineOwnedByEntity()
            {
                // Arrange
                var entity = _scene.CreateEntity();
                var coroutine = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), entity, UpdateMode);

                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

                // Act
                entity.RemoveAfterFullFrame();
                _scene.RemoveEntitiesAfterFullFrame();
                ProcessCoroutines();

                // Assert
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            [Test]
            public void RemoveEntity_ShouldNotChangeStateOfCompletedCoroutineOwnedByEntity_WhenEntityRemovedBeforeNextProcessCoroutines()
            {
                // Arrange
                var entity = _scene.CreateEntity();
                var coroutine1 = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), entity, UpdateMode);
                var coroutine2 = _coroutineSystem.StartCoroutine(RemoveEntityCoroutine(entity), UpdateMode);

                // Act
                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
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
                var coroutine = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), component, UpdateMode);

                // Act
                entity.RemoveComponent(component);
                ProcessCoroutines();

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
                var coroutine1 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), component, UpdateMode);
                var coroutine2 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), component, UpdateMode);
                var coroutine3 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), component, UpdateMode);

                // Act
                entity.RemoveComponent(component);
                ProcessCoroutines();

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
                var coroutine1 = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), component, UpdateMode);
                var coroutine2 = _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(new Data()), component, UpdateMode);

                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                // Assume
                Assert.That(coroutine1.State, Is.EqualTo(CoroutineState.Completed));

                // Act
                entity.RemoveComponent(component);
                ProcessCoroutines();

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
                var coroutine = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), component, UpdateMode);

                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                // Assume
                Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Completed));

                // Act
                entity.RemoveComponent(component);
                ProcessCoroutines();

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
                var coroutine1 = _coroutineSystem.StartCoroutine(CompleteAfterFirstRunCoroutine(), component, UpdateMode);
                var coroutine2 = _coroutineSystem.StartCoroutine(RemoveComponentCoroutine(component), UpdateMode);

                // Act
                for (var i = 0; i < 10; i++)
                {
                    ProcessCoroutines();
                }

                // Assert
                Assert.That(coroutine1.State, Is.EqualTo(CoroutineState.Completed));
                Assert.That(coroutine2.State, Is.EqualTo(CoroutineState.Completed));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.Zero);
            }

            #endregion

            private IEnumerator<CoroutineInstruction> StartAnotherCoroutine(Data data)
            {
                data.Log.Add($"{nameof(StartAnotherCoroutine)} - 1");
                yield return Coroutine.WaitForNextFrame();

                _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(data), UpdateMode);
                data.Log.Add($"{nameof(StartAnotherCoroutine)} - 2");
                yield return Coroutine.WaitForNextFrame();

                data.Log.Add($"{nameof(StartAnotherCoroutine)} - 3");
                yield return Coroutine.WaitForNextFrame();

                data.Log.Add($"{nameof(StartAnotherCoroutine)} - 4");
                yield return Coroutine.WaitForNextFrame();
            }
        }

        #endregion

        #region Additional tests for variable time step

        [TestFixture]
        public class VariableTimeStepCoroutineSystemTests : CommonCoroutineSystemTests
        {
            private readonly GameTime _deltaTime = new(TimeSpan.FromMilliseconds(16));

            protected override CoroutineUpdateMode UpdateMode => CoroutineUpdateMode.VariableTimeStep;

            protected override void ProcessCoroutines()
            {
                _coroutineSystem.ProcessCoroutines(_deltaTime);
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
                _coroutineSystem.StartCoroutine(WaitTimeCoroutine(data), UpdateMode);

                // Act
                for (var i = 0; i < executionTimes; i++)
                {
                    _coroutineSystem.ProcessCoroutines(new GameTime(TimeSpan.FromMilliseconds(deltaTime)));
                }

                // Assert
                Assert.That(data.Log.Count, Is.EqualTo(expectedProgressCount));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }
        }

        #endregion

        #region Additional tests for fixed time step

        [TestFixture]
        public class FixedTimeStepCoroutineSystemTests : CommonCoroutineSystemTests
        {
            protected override CoroutineUpdateMode UpdateMode => CoroutineUpdateMode.FixedTimeStep;

            protected override void ProcessCoroutines()
            {
                _coroutineSystem.ProcessCoroutines();
            }

            [TestCase(1, 40, 1)]
            [TestCase(2, 40, 1)]
            [TestCase(3, 40, 1)]
            [TestCase(4, 40, 2)]
            [TestCase(12, 40, 4)]
            [TestCase(3, 100, 3)]
            [TestCase(3, 200, 3)]
            public void ProcessCoroutines_ShouldProgressCoroutineCorrectly_GivenCoroutineWithWaitTime100MS(int executionTimes, int fixedDeltaTime,
                int expectedProgressCount)
            {
                // Arrange
                var data = new Data();
                _coroutineSystem.StartCoroutine(WaitTimeCoroutine(data), UpdateMode);

                GameTime.FixedDeltaTime = TimeSpan.FromMilliseconds(fixedDeltaTime);

                // Act
                for (var i = 0; i < executionTimes; i++)
                {
                    _coroutineSystem.ProcessCoroutines();
                }

                // Assert
                Assert.That(data.Log.Count, Is.EqualTo(expectedProgressCount));
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
            }
        }

        #endregion

        #region Additional tests

        [TestFixture]
        public class AdditionalCoroutineSystemTests : CoroutineSystemTests
        {
            private readonly GameTime _deltaTime = new(TimeSpan.FromMilliseconds(16));

            [Test]
            public void ProcessCoroutines_ShouldThrowException_WhenFixedTimeStepCoroutineSwitchesToVariableTimeStepCoroutine()
            {
                // Arrange
                var data = new Data();
                var coroutine = _coroutineSystem.CreateCoroutine(UpdateEveryFrameCoroutine(data), CoroutineUpdateMode.VariableTimeStep);
                _coroutineSystem.StartCoroutine(SwitchToCoroutine(data, coroutine), CoroutineUpdateMode.FixedTimeStep);

                // Act
                // Assert
                Assert.That(() => _coroutineSystem.ProcessCoroutines(),
                    Throws.InvalidOperationException.With.Message.EqualTo("Cannot switch to coroutine with different update mode."));
            }

            [Test]
            public void ProcessCoroutines_ShouldThrowException_WhenVariableTimeStepCoroutineSwitchesToFixedTimeStepCoroutine()
            {
                // Arrange
                var data = new Data();
                var coroutine = _coroutineSystem.CreateCoroutine(UpdateEveryFrameCoroutine(data), CoroutineUpdateMode.FixedTimeStep);
                _coroutineSystem.StartCoroutine(SwitchToCoroutine(data, coroutine), CoroutineUpdateMode.VariableTimeStep);

                // Act
                // Assert
                Assert.That(() => _coroutineSystem.ProcessCoroutines(_deltaTime),
                    Throws.InvalidOperationException.With.Message.EqualTo("Cannot switch to coroutine with different update mode."));
            }

            [Test]
            public void ProcessCoroutines_FixedTimeStep_ShouldExecuteFixedTimeStepCoroutinesOnly()
            {
                // Arrange
                var variableTimeStepData = new Data();
                var fixedTimeStepData = new Data();
                _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(variableTimeStepData), CoroutineUpdateMode.VariableTimeStep);
                _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(fixedTimeStepData), CoroutineUpdateMode.FixedTimeStep);

                // Act
                _coroutineSystem.ProcessCoroutines();

                // Assert
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
                Assert.That(variableTimeStepData.Log, Is.Empty);
                Assert.That(fixedTimeStepData.Log, Is.EqualTo(new[]
                {
                    "UpdateEveryFrameCoroutine - 1"
                }));
            }

            [Test]
            public void ProcessCoroutines_VariableTimeStep_ShouldExecuteVariableTimeStepCoroutinesOnly()
            {
                // Arrange
                var variableTimeStepData = new Data();
                var fixedTimeStepData = new Data();
                _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(variableTimeStepData), CoroutineUpdateMode.VariableTimeStep);
                _coroutineSystem.StartCoroutine(UpdateEveryFrameCoroutine(fixedTimeStepData), CoroutineUpdateMode.FixedTimeStep);

                // Act
                _coroutineSystem.ProcessCoroutines(_deltaTime);

                // Assert
                Assert.That(_coroutineSystem.ActiveCoroutinesCount, Is.EqualTo(1));
                Assert.That(variableTimeStepData.Log, Is.EqualTo(new[]
                {
                    "UpdateEveryFrameCoroutine - 1"
                }));
                Assert.That(fixedTimeStepData.Log, Is.Empty);
            }
        }

        #endregion
    }
}