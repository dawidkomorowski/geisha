using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Systems
{
    [TestFixture]
    public class SystemsProviderTests
    {
        private ISystem _system1;
        private ISystem _system2;
        private ISystem _system3;
        private Scene _scene;

        private IList<ISystem> Systems => new List<ISystem> {_system1, _system2, _system3};

        [SetUp]
        public void SetUp()
        {
            _system1 = Substitute.For<ISystem>();
            _system2 = Substitute.For<ISystem>();
            _system3 = Substitute.For<ISystem>();
            _scene = new Scene();
        }

        [Test]
        public void GetSystemsUpdatableForScene_ShouldReturnUpdatableWithSystemsGivenInConstructor()
        {
            // Arrange
            var systemsProvider = new SystemsProvider(Systems);

            // Act
            var updateList = (UpdateList) systemsProvider.GetSystemsUpdatableForScene(_scene);

            // Assert
            CollectionAssert.AreEquivalent(Systems, updateList.Updatables);
        }

        [Test]
        public void GetSystemsUpdatableForScene_ShouldSetSceneForAllSystemsGivenInConstructor()
        {
            // Arrange
            var systemsProvider = new SystemsProvider(Systems);

            // Act
            var updateList = (UpdateList) systemsProvider.GetSystemsUpdatableForScene(_scene);

            // Assert
            var system1 = (ISystem) updateList.Updatables[0];
            var system2 = (ISystem) updateList.Updatables[1];
            var system3 = (ISystem) updateList.Updatables[2];

            Assert.That(system1.Scene, Is.EqualTo(_scene));
            Assert.That(system2.Scene, Is.EqualTo(_scene));
            Assert.That(system3.Scene, Is.EqualTo(_scene));
        }

        [Test]
        public void GetSystemsUpdatableForScene_ShouldReturnUpdatableWithSystemsGivenInConstructorOrderedByPriority()
        {
            // Arrange
            _system1.Priority = 2;
            _system2.Priority = 3;
            _system3.Priority = 1;
            var systemsProvider = new SystemsProvider(Systems);

            // Act
            var updateList = (UpdateList) systemsProvider.GetSystemsUpdatableForScene(_scene);

            // Assert
            var priorities = updateList.Updatables.OfType<ISystem>().Select(s => s.Priority).ToList();
            CollectionAssert.IsOrdered(priorities);
        }
    }
}