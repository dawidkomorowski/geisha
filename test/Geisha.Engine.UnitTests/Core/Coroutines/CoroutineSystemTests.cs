using System;
using System.Collections.Generic;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Coroutines;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Coroutines
{
    [TestFixture]
    public class CoroutineSystemTests
    {
        private readonly GameTime _deltaTime = new(TimeSpan.FromMilliseconds(16));
        private CoroutineSystem _coroutineSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _coroutineSystem = new CoroutineSystem();
        }

        [Test]
        public void NewCoroutine_ShouldHaveStatePending()
        {
            // Arrange
            // Act
            var coroutine = Coroutine.Create(WaitForNextFrameCoroutine(new Data()));

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));
        }

        [Test]
        public void StartCoroutine_ShouldNotExecuteCoroutine()
        {
            // Arrange
            var data = new Data();

            // Act
            _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(data));

            // Assert
            Assert.That(data.Number, Is.Zero);
        }

        [Test]
        public void StartCoroutine_ShouldReturnCoroutineInRunningState()
        {
            // Arrange
            // Act
            var coroutine = _coroutineSystem.StartCoroutine(WaitForNextFrameCoroutine(new Data()));

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
        }

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
        }

        [Test]
        public void ProcessCoroutines_ShouldNotExecuteCoroutines_WhenTopLevelCoroutineIsPaused_GivenCoroutineWithCall()
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
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutines_WhenTopLevelCoroutineIsPausedAndThenResumed_GivenCoroutineWithCall()
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
        }

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
        }

        [Test]
        public void Coroutine_Pause_ShouldThrowException_WhenCoroutineIsInPendingState()
        {
            // Arrange
            var data = new Data();
            var coroutine = Coroutine.Create(WaitForNextFrameCoroutine(data));

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
        }

        [Test]
        public void Coroutine_Resume_ShouldThrowException_WhenCoroutineIsInPendingState()
        {
            // Arrange
            var data = new Data();
            var coroutine = Coroutine.Create(WaitForNextFrameCoroutine(data));

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

        private sealed class Data
        {
            public int Number { get; set; }
            public bool Condition { get; set; }
            public List<string> Log { get; } = new();
        }
    }
}