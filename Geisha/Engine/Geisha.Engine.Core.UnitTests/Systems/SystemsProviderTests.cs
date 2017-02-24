using System.Collections.Generic;
using System.Linq;
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

        private IList<ISystem> Systems => new List<ISystem> {_system1, _system2, _system3};

        [SetUp]
        public void SetUp()
        {
            _system1 = Substitute.For<ISystem>();
            _system2 = Substitute.For<ISystem>();
            _system3 = Substitute.For<ISystem>();
        }

        [Test]
        public void GetSystems_ShouldReturnEnumerableWithSystemsGivenInConstructor()
        {
            // Arrange
            var systemsProvider = new SystemsProvider(Systems);

            // Act
            var systems = systemsProvider.GetSystems();

            // Assert
            CollectionAssert.AreEquivalent(Systems, systems);
        }

        [Test]
        public void GetSystems_ShouldReturnEnumerableWithSystemsGivenInConstructorOrderedByPriority()
        {
            // Arrange
            _system1.Priority = 2;
            _system2.Priority = 3;
            _system3.Priority = 1;
            var systemsProvider = new SystemsProvider(Systems);

            // Act
            var systems = systemsProvider.GetSystems();

            // Assert
            var priorities = systems.Select(s => s.Priority).ToList();
            CollectionAssert.IsOrdered(priorities);
        }
    }
}