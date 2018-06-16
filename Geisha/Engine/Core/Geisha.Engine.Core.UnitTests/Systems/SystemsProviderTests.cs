using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Systems
{
    [TestFixture]
    public class SystemsProviderTests
    {
        private CoreConfiguration _coreConfiguration;
        private IConfigurationManager _configurationManager;

        private IVariableTimeStepSystem _variableTimeStepSystem1;
        private IVariableTimeStepSystem _variableTimeStepSystem2;
        private IVariableTimeStepSystem _variableTimeStepSystem3;
        private IFixedTimeStepSystem _fixedTimeStepSystem1;
        private IFixedTimeStepSystem _fixedTimeStepSystem2;
        private IFixedTimeStepSystem _fixedTimeStepSystem3;

        private IList<IVariableTimeStepSystem> VariableTimeStepSystems =>
            new List<IVariableTimeStepSystem> {_variableTimeStepSystem1, _variableTimeStepSystem2, _variableTimeStepSystem3};

        private IList<IFixedTimeStepSystem> FixedTimeStepSystems =>
            new List<IFixedTimeStepSystem> {_fixedTimeStepSystem1, _fixedTimeStepSystem2, _fixedTimeStepSystem3};

        [SetUp]
        public void SetUp()
        {
            _coreConfiguration = new CoreConfiguration();
            _configurationManager = Substitute.For<IConfigurationManager>();
            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(_coreConfiguration);

            _variableTimeStepSystem1 = Substitute.For<IVariableTimeStepSystem>();
            _variableTimeStepSystem2 = Substitute.For<IVariableTimeStepSystem>();
            _variableTimeStepSystem3 = Substitute.For<IVariableTimeStepSystem>();
            _fixedTimeStepSystem1 = Substitute.For<IFixedTimeStepSystem>();
            _fixedTimeStepSystem2 = Substitute.For<IFixedTimeStepSystem>();
            _fixedTimeStepSystem3 = Substitute.For<IFixedTimeStepSystem>();
        }

        private ISystemsProvider GetSystemsProvider()
        {
            return new SystemsProvider(_configurationManager, FixedTimeStepSystems, VariableTimeStepSystems);
        }

        [Test]
        public void GetFixedTimeStepSystems_ShouldReturnFixedTimeStepSystemsPresentInConfiguration()
        {
            // Arrange
            var systemsProvider = GetSystemsProvider();

            // Act
            var systems = systemsProvider.GetFixedTimeStepSystems();

            // Assert
            Assert.That(systems, Is.EquivalentTo(FixedTimeStepSystems));
        }

        [Test]
        public void GetFixedTimeStepSystems_ShouldReturnSystemsOrderedByPriority()
        {
            // Arrange
            var systemsProvider = GetSystemsProvider();

            _fixedTimeStepSystem1.Priority = 2;
            _fixedTimeStepSystem2.Priority = 3;
            _fixedTimeStepSystem3.Priority = 1;

            // Act
            var systems = systemsProvider.GetFixedTimeStepSystems();

            // Assert
            var priorities = systems.Select(s => s.Priority).ToList();
            Assert.That(priorities, Is.Ordered);
        }

        [Test]
        public void GetVariableTimeStepSystems_ShouldReturnSystemsOrderedByPriority()
        {
            // Arrange
            var systemsProvider = GetSystemsProvider();

            _variableTimeStepSystem1.Priority = 2;
            _variableTimeStepSystem2.Priority = 3;
            _variableTimeStepSystem3.Priority = 1;

            // Act
            var systems = systemsProvider.GetVariableTimeStepSystems();

            // Assert
            var priorities = systems.Select(s => s.Priority).ToList();
            Assert.That(priorities, Is.Ordered);
        }

        [Test]
        public void GetVariableTimeStepSystems_ShouldReturnVariableTimeStepSystems()
        {
            // Arrange
            var systemsProvider = GetSystemsProvider();

            // Act
            var systems = systemsProvider.GetVariableTimeStepSystems();

            // Assert
            Assert.That(systems, Is.EquivalentTo(VariableTimeStepSystems));
        }
    }
}