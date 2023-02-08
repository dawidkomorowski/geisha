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
        private readonly GameTime DeltaTime = new(TimeSpan.FromMilliseconds(16));
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
            var coroutine = Coroutine.Create(IncrementNumber(new Data()));

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Pending));
        }

        [Test]
        public void StartCoroutine_ShouldNotExecuteCoroutine()
        {
            // Arrange
            var data = new Data();

            // Act
            _coroutineSystem.StartCoroutine(IncrementNumber(data));

            // Assert
            Assert.That(data.Number, Is.Zero);
        }

        [Test]
        public void StartCoroutine_ShouldReturnCoroutineInRunningState()
        {
            // Arrange
            // Act
            var coroutine = _coroutineSystem.StartCoroutine(IncrementNumber(new Data()));

            // Assert
            Assert.That(coroutine.State, Is.EqualTo(CoroutineState.Running));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutineToFirstYield_WhenCalledOnce()
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(IncrementNumber(data));

            // Act
            _coroutineSystem.ProcessCoroutines(DeltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(1));
        }

        [Test]
        public void StartCoroutine_ShouldExecuteCoroutineToSecondYield_WhenCalledTwice()
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(IncrementNumber(data));

            // Act
            _coroutineSystem.ProcessCoroutines(DeltaTime);
            _coroutineSystem.ProcessCoroutines(DeltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(2));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteCoroutineTillCompletion_WhenExecutedManyTimes()
        {
            // Arrange
            var data = new Data();
            _coroutineSystem.StartCoroutine(IncrementNumber(data));

            // Act
            for (var i = 0; i < 10; i++)
            {
                _coroutineSystem.ProcessCoroutines(DeltaTime);
            }

            // Assert
            Assert.That(data.Number, Is.EqualTo(3));
        }

        [Test]
        public void ProcessCoroutines_ShouldExecuteMultipleCoroutines()
        {
            // Arrange
            var data1 = new Data();
            var data2 = new Data();
            var data3 = new Data();

            _coroutineSystem.StartCoroutine(IncrementNumber(data1));
            _coroutineSystem.StartCoroutine(IncrementNumber(data2));
            _coroutineSystem.StartCoroutine(IncrementNumber(data3));

            // Act
            _coroutineSystem.ProcessCoroutines(DeltaTime);

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

            _coroutineSystem.StartCoroutine(IncrementNumber(data1));
            _coroutineSystem.ProcessCoroutines(DeltaTime);
            _coroutineSystem.StartCoroutine(IncrementNumber(data2));
            _coroutineSystem.ProcessCoroutines(DeltaTime);
            _coroutineSystem.StartCoroutine(IncrementNumber(data3));

            // Act
            _coroutineSystem.ProcessCoroutines(DeltaTime);

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
        public void Coroutine_Stop_ShouldStopCoroutineFromBeingExecuted()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(IncrementNumber(data));
            _coroutineSystem.ProcessCoroutines(DeltaTime);

            // Act
            coroutine.Stop();
            _coroutineSystem.ProcessCoroutines(DeltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(1));
        }

        [Test]
        public void Coroutine_Pause_ShouldPauseCoroutineFromBeingExecuted()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(IncrementNumber(data));
            _coroutineSystem.ProcessCoroutines(DeltaTime);

            // Act
            coroutine.Pause();
            _coroutineSystem.ProcessCoroutines(DeltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(1));
        }

        [Test]
        public void Coroutine_Resume_ShouldResumePausedCoroutineSoItContinuesExecuting()
        {
            // Arrange
            var data = new Data();
            var coroutine = _coroutineSystem.StartCoroutine(IncrementNumber(data));
            _coroutineSystem.ProcessCoroutines(DeltaTime);
            coroutine.Pause();
            _coroutineSystem.ProcessCoroutines(DeltaTime);

            // Act
            coroutine.Resume();
            _coroutineSystem.ProcessCoroutines(DeltaTime);

            // Assert
            Assert.That(data.Number, Is.EqualTo(2));
        }

        private static IEnumerator<CoroutineInstruction> IncrementNumber(Data data)
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
        }

        private sealed class Data
        {
            public int Number { get; set; }
        }
    }
}