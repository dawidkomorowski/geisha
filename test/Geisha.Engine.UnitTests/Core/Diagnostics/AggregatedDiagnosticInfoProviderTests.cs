using System;
using System.Linq;
using Geisha.Engine.Core.Diagnostics;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
{
    [TestFixture]
    public class AggregatedDiagnosticInfoProviderTests
    {
        [Test]
        public void Initialize_ShouldThrowException_WhenAlreadyInitialized()
        {
            // Arrange
            var provider1 = Substitute.For<IDiagnosticInfoProvider>();
            var provider2 = Substitute.For<IDiagnosticInfoProvider>();
            var provider3 = Substitute.For<IDiagnosticInfoProvider>();

            var aggregatedDiagnosticInfoProvider = new AggregatedDiagnosticInfoProvider();
            aggregatedDiagnosticInfoProvider.Initialize(new[] { provider1, provider2, provider3 });

            // Act
            // Assert
            Assert.That(() => aggregatedDiagnosticInfoProvider.Initialize(new[] { provider1, provider2, provider3 }), Throws.InvalidOperationException);
        }

        [Test]
        public void GetAllDiagnosticInfo_ShouldThrowException_WhenNotInitialized()
        {
            // Arrange
            var aggregatedDiagnosticInfoProvider = new AggregatedDiagnosticInfoProvider();

            // Act
            // Assert
            Assert.That(() => aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo(), Throws.InvalidOperationException);
        }

        [Test]
        public void GetAllDiagnosticInfo_ShouldReturnEmptyEnumerable_WhenInitializedWithNoProvider()
        {
            // Arrange
            var aggregatedDiagnosticInfoProvider = new AggregatedDiagnosticInfoProvider();
            aggregatedDiagnosticInfoProvider.Initialize(Enumerable.Empty<IDiagnosticInfoProvider>());

            // Act
            var actual = aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetAllDiagnosticInfo_ShouldReturnDiagnosticInfoFromAllProviders()
        {
            // Arrange
            var diagnosticInfo1 = GetRandomDiagnosticInfo();
            var diagnosticInfo2 = GetRandomDiagnosticInfo();
            var diagnosticInfo3 = GetRandomDiagnosticInfo();
            var diagnosticInfo4 = GetRandomDiagnosticInfo();
            var diagnosticInfo5 = GetRandomDiagnosticInfo();

            var provider1 = Substitute.For<IDiagnosticInfoProvider>();
            var provider2 = Substitute.For<IDiagnosticInfoProvider>();
            var provider3 = Substitute.For<IDiagnosticInfoProvider>();

            provider1.GetDiagnosticInfo().Returns(new[] { diagnosticInfo1, diagnosticInfo2 });
            provider2.GetDiagnosticInfo().Returns(new[] { diagnosticInfo3, diagnosticInfo4 });
            provider3.GetDiagnosticInfo().Returns(new[] { diagnosticInfo5 });

            var aggregatedDiagnosticInfoProvider = new AggregatedDiagnosticInfoProvider();
            aggregatedDiagnosticInfoProvider.Initialize(new[] { provider1, provider2, provider3 });

            // Act
            var actual = aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo();

            // Assert
            Assert.That(actual, Is.EquivalentTo(new[] { diagnosticInfo1, diagnosticInfo2, diagnosticInfo3, diagnosticInfo4, diagnosticInfo5 }));
        }

        private static DiagnosticInfo GetRandomDiagnosticInfo()
        {
            return new DiagnosticInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }
    }
}