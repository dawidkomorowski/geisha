using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Systems
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

            _variableTimeStepSystem1.Name.Returns(Guid.NewGuid().ToString());
            _variableTimeStepSystem2.Name.Returns(Guid.NewGuid().ToString());
            _variableTimeStepSystem3.Name.Returns(Guid.NewGuid().ToString());
            _fixedTimeStepSystem1.Name.Returns(Guid.NewGuid().ToString());
            _fixedTimeStepSystem2.Name.Returns(Guid.NewGuid().ToString());
            _fixedTimeStepSystem3.Name.Returns(Guid.NewGuid().ToString());
        }

        private ISystemsProvider GetSystemsProvider()
        {
            return new SystemsProvider(_configurationManager, FixedTimeStepSystems, VariableTimeStepSystems);
        }

        [Test]
        public void GetFixedTimeStepSystems_ShouldReturnFixedTimeStepSystemsPresentInConfiguration()
        {
            // Arrange
            _coreConfiguration.SystemsExecutionChain.Add(_fixedTimeStepSystem1.Name);
            _coreConfiguration.SystemsExecutionChain.Add(_fixedTimeStepSystem2.Name);

            var systemsProvider = GetSystemsProvider();

            // Act
            var systems = systemsProvider.GetFixedTimeStepSystems();

            // Assert
            Assert.That(systems, Is.EquivalentTo(new[] {_fixedTimeStepSystem1, _fixedTimeStepSystem2}));
        }

        [Test]
        public void GetFixedTimeStepSystems_ShouldReturnSystemsOrderedByPriority()
        {
            // Arrange
            _coreConfiguration.SystemsExecutionChain.Add(_fixedTimeStepSystem3.Name);
            _coreConfiguration.SystemsExecutionChain.Add(_fixedTimeStepSystem1.Name);
            _coreConfiguration.SystemsExecutionChain.Add(_fixedTimeStepSystem2.Name);

            var systemsProvider = GetSystemsProvider();

            // Act
            var systems = systemsProvider.GetFixedTimeStepSystems().ToList();

            // Assert
            Assert.That(systems[0], Is.EqualTo(_fixedTimeStepSystem3));
            Assert.That(systems[1], Is.EqualTo(_fixedTimeStepSystem1));
            Assert.That(systems[2], Is.EqualTo(_fixedTimeStepSystem2));
        }

        [Test]
        public void GetVariableTimeStepSystems_ShouldReturnVariableTimeStepSystemsPresentInConfiguration()
        {
            // Arrange
            _coreConfiguration.SystemsExecutionChain.Add(_variableTimeStepSystem1.Name);
            _coreConfiguration.SystemsExecutionChain.Add(_variableTimeStepSystem2.Name);

            var systemsProvider = GetSystemsProvider();

            // Act
            var systems = systemsProvider.GetVariableTimeStepSystems();

            // Assert
            Assert.That(systems, Is.EquivalentTo(new[] {_variableTimeStepSystem1, _variableTimeStepSystem2}));
        }

        [Test]
        public void GetVariableTimeStepSystems_ShouldReturnSystemsOrderedByPriority()
        {
            // Arrange
            _coreConfiguration.SystemsExecutionChain.Add(_variableTimeStepSystem3.Name);
            _coreConfiguration.SystemsExecutionChain.Add(_variableTimeStepSystem1.Name);
            _coreConfiguration.SystemsExecutionChain.Add(_variableTimeStepSystem2.Name);

            var systemsProvider = GetSystemsProvider();

            // Act
            var systems = systemsProvider.GetVariableTimeStepSystems().ToList();

            // Assert
            Assert.That(systems[0], Is.EqualTo(_variableTimeStepSystem3));
            Assert.That(systems[1], Is.EqualTo(_variableTimeStepSystem1));
            Assert.That(systems[2], Is.EqualTo(_variableTimeStepSystem2));
        }
    }
}