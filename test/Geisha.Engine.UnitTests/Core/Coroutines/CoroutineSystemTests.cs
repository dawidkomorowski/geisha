using System.Collections.Generic;
using Geisha.Engine.Core.Coroutines;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Coroutines
{
    [TestFixture]
    public class CoroutineSystemTests
    {
        private CoroutineSystem _coroutineSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _coroutineSystem = new CoroutineSystem();
        }

        [Test]
        public void StartCoroutine_Test()
        {
            // Arrange
            _incrementNumberValue = 0;
            _coroutineSystem.StartCoroutine(IncrementNumber());

            // Act
            _coroutineSystem.ProcessCoroutines();

            // Assert
            Assert.That(_incrementNumberValue, Is.EqualTo(1));
        }

        [Test]
        public void StartCoroutine_Test2()
        {
            // Arrange
            _incrementNumberValue = 0;
            _coroutineSystem.StartCoroutine(IncrementNumber());

            // Act
            _coroutineSystem.ProcessCoroutines();
            _coroutineSystem.ProcessCoroutines();
            _coroutineSystem.ProcessCoroutines();

            // Assert
            Assert.That(_incrementNumberValue, Is.EqualTo(3));
        }

        private int _incrementNumberValue = 0;
        private IEnumerator<Coroutine> IncrementNumber()
        {
            _incrementNumberValue++;
            yield return new Coroutine();

            _incrementNumberValue++;
            yield return new Coroutine();

            _incrementNumberValue++;
            yield return new Coroutine();
        }
    }
}