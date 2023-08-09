using System.Linq;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Diagnostics;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Diagnostics;

[TestFixture]
public class RenderingDiagnosticInfoProviderTests
{
    [Test]
    public void GetDiagnosticInfo_ShouldReturnRenderingStatistics_WhenEnabledInConfiguration()
    {
        // Arrange
        var renderingConfiguration = new RenderingConfiguration
        {
            ShowRenderingStatistics = true
        };

        var diagnosticInfoProvider = new RenderingDiagnosticInfoProvider(renderingConfiguration);

        var renderingStatistics = new RenderingStatistics
        {
            DrawCalls = 12
        };

        diagnosticInfoProvider.UpdateDiagnostics(renderingStatistics);

        // Act
        var actual = diagnosticInfoProvider.GetDiagnosticInfo().ToList();

        // Assert
        Assert.That(actual, Has.Count.EqualTo(1));
        Assert.That(actual[0].Name, Is.EqualTo("DrawCalls"));
        Assert.That(actual[0].Value, Is.EqualTo("12"));
    }

    [Test]
    public void GetDiagnosticInfo_ShouldNotReturnRenderingStatistics_WhenDisabledInConfiguration()
    {
        // Arrange
        var renderingConfiguration = new RenderingConfiguration
        {
            ShowRenderingStatistics = false
        };

        var diagnosticInfoProvider = new RenderingDiagnosticInfoProvider(renderingConfiguration);

        var renderingStatistics = new RenderingStatistics
        {
            DrawCalls = 12
        };

        diagnosticInfoProvider.UpdateDiagnostics(renderingStatistics);

        // Act
        var actual = diagnosticInfoProvider.GetDiagnosticInfo().ToList();

        // Assert
        Assert.That(actual, Is.Empty);
    }
}