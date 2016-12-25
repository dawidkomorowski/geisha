using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class UpdateListTests
    {
        private IUpdatable _updatable1;
        private IUpdatable _updatable2;
        private IUpdatable _updatable3;

        private IEnumerable<IUpdatable> Updatables => new List<IUpdatable> {_updatable1, _updatable2, _updatable3};

        [SetUp]
        public void SetUp()
        {
            _updatable1 = Substitute.For<IUpdatable>();
            _updatable2 = Substitute.For<IUpdatable>();
            _updatable3 = Substitute.For<IUpdatable>();
        }

        [Test]
        public void Updatables_ShouldReturnListGivenInConstructorInTheSameOrder()
        {
            // Arrange
            var updateList = new UpdateList(Updatables);

            // Act
            var updatables = updateList.Updatables;

            // Assert
            CollectionAssert.AreEqual(Updatables, updatables);
        }

        [Test]
        public void Update_ShouldCallUpdateOfAllUpdatablesGivenInConstructorInCorrectOrderWithCorrectDeltaTimeValue()
        {
            // Arrange
            var updateList = new UpdateList(Updatables);
            const double deltaTime = 0.1;

            // Act
            updateList.Update(deltaTime);

            // Assert
            Received.InOrder(() =>
            {
                _updatable1.Update(deltaTime);
                _updatable2.Update(deltaTime);
                _updatable3.Update(deltaTime);
            });
        }

        [Test]
        public void Update_ShouldCallFixedUpdateOfAllUpdatablesGivenInConstructorInCorrectOrder()
        {
            // Arrange
            var updateList = new UpdateList(Updatables);

            // Act
            updateList.FixedUpdate();

            // Assert
            Received.InOrder(() =>
            {
                _updatable1.FixedUpdate();
                _updatable2.FixedUpdate();
                _updatable3.FixedUpdate();
            });
        }
    }
}