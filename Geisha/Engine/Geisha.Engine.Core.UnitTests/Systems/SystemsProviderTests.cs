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
        public void GetVariableUpdateSystems_ShouldReturnSystemsWithUpdateMode_Variable_and_Both()
        {
            // Arrange
            var systemsProvider = new SystemsProvider(Systems);

            _system1.UpdateMode = UpdateMode.Variable;
            _system2.UpdateMode = UpdateMode.Fixed;
            _system3.UpdateMode = UpdateMode.Both;

            // Act
            var systems = systemsProvider.GetVariableUpdateSystems();

            // Assert
            CollectionAssert.AreEquivalent(new[] {_system1, _system3}, systems);
        }

        [Test]
        public void GetVariableUpdateSystems_ShouldReturnSystemsOrderedByPriority()
        {
            // Arrange
            var systemsProvider = new SystemsProvider(Systems);

            _system1.Priority = 2;
            _system2.Priority = 3;
            _system3.Priority = 1;

            _system1.UpdateMode = UpdateMode.Variable;
            _system2.UpdateMode = UpdateMode.Variable;
            _system3.UpdateMode = UpdateMode.Variable;

            // Act
            var systems = systemsProvider.GetVariableUpdateSystems();

            // Assert
            var priorities = systems.Select(s => s.Priority).ToList();
            CollectionAssert.IsOrdered(priorities);
        }

        [Test]
        public void GetFixedUpdateSystems_ShouldReturnSystemsWithUpdateMode_Fixed_and_Both()
        {
            // Arrange
            var systemsProvider = new SystemsProvider(Systems);

            _system1.UpdateMode = UpdateMode.Variable;
            _system2.UpdateMode = UpdateMode.Fixed;
            _system3.UpdateMode = UpdateMode.Both;

            // Act
            var systems = systemsProvider.GetFixedUpdateSystems();

            // Assert
            CollectionAssert.AreEquivalent(new[] {_system2, _system3}, systems);
        }

        [Test]
        public void GetFixedUpdateSystems_ShouldReturnSystemsOrderedByPriority()
        {
            // Arrange
            var systemsProvider = new SystemsProvider(Systems);

            _system1.Priority = 2;
            _system2.Priority = 3;
            _system3.Priority = 1;

            _system1.UpdateMode = UpdateMode.Fixed;
            _system2.UpdateMode = UpdateMode.Fixed;
            _system3.UpdateMode = UpdateMode.Fixed;

            // Act
            var systems = systemsProvider.GetFixedUpdateSystems();

            // Assert
            var priorities = systems.Select(s => s.Priority).ToList();
            CollectionAssert.IsOrdered(priorities);
        }
    }
}