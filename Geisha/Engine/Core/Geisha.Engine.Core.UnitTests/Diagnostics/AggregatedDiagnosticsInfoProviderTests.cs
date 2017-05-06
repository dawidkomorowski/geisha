using System;
using Geisha.Engine.Core.Diagnostics;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Diagnostics
{
    [TestFixture]
    public class AggregatedDiagnosticsInfoProviderTests
    {
        [Test]
        public void GetDiagnosticsInfo_ShouldReturnDiagnosticsInfoFromAllProviders()
        {
            // Arrange
            var diagnosticsInfo1 = GetRandomDiagnosticsInfo();
            var diagnosticsInfo2 = GetRandomDiagnosticsInfo();
            var diagnosticsInfo3 = GetRandomDiagnosticsInfo();
            var diagnosticsInfo4 = GetRandomDiagnosticsInfo();
            var diagnosticsInfo5 = GetRandomDiagnosticsInfo();

            var provider1 = Substitute.For<IDiagnosticsInfoProvider>();
            var provider2 = Substitute.For<IDiagnosticsInfoProvider>();
            var provider3 = Substitute.For<IDiagnosticsInfoProvider>();

            provider1.GetDiagnosticsInfo().Returns(new[] {diagnosticsInfo1, diagnosticsInfo2});
            provider2.GetDiagnosticsInfo().Returns(new[] {diagnosticsInfo3, diagnosticsInfo4});
            provider3.GetDiagnosticsInfo().Returns(new[] {diagnosticsInfo5});

            var aggregatedDiagnosticsInfoProvider = new AggregatedDiagnosticsInfoProvider(new[] {provider1, provider2, provider3});

            // Act
            var actual = aggregatedDiagnosticsInfoProvider.GetDiagnosticsInfo();

            // Assert
            Assert.That(actual, Is.EquivalentTo(new[] {diagnosticsInfo1, diagnosticsInfo2, diagnosticsInfo3, diagnosticsInfo4, diagnosticsInfo5}));
        }

        private static DiagnosticsInfo GetRandomDiagnosticsInfo()
        {
            return new DiagnosticsInfo
            {
                Name = Guid.NewGuid().ToString(),
                Value = Guid.NewGuid().ToString()
            };
        }
    }
}