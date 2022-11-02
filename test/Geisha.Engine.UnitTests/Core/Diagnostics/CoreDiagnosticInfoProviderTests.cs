using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Diagnostics;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
{
    [TestFixture]
    public class CoreDiagnosticInfoProviderTests
    {
        private const string StepName1 = "Step 1";
        private const string StepName2 = "Step 2";
        private const string StepName3 = "Step 3";

        private IPerformanceStatisticsProvider _performanceStatisticsProvider = null!;
        private GameLoopStepStatistics _stepStatistics1 = null!;
        private GameLoopStepStatistics _stepStatistics2 = null!;
        private GameLoopStepStatistics _stepStatistics3 = null!;

        [SetUp]
        public void SetUp()
        {
            _performanceStatisticsProvider = Substitute.For<IPerformanceStatisticsProvider>();

            _stepStatistics1 = new GameLoopStepStatistics(StepName1, TimeSpan.FromMilliseconds(8), 0.1);
            _stepStatistics2 = new GameLoopStepStatistics(StepName2, TimeSpan.FromMilliseconds(16), 0.2);
            _stepStatistics3 = new GameLoopStepStatistics(StepName3, TimeSpan.FromMilliseconds(33), 0.3);

            _performanceStatisticsProvider.GetGameLoopStatistics().Returns(new[]
            {
                _stepStatistics1, _stepStatistics2, _stepStatistics3
            });
        }

        private CoreDiagnosticInfoProvider GetCoreDiagnosticInfoProvider(CoreConfiguration configuration)
        {
            return new CoreDiagnosticInfoProvider(configuration, _performanceStatisticsProvider);
        }

        private CoreDiagnosticInfoProvider GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled()
        {
            var configuration = new CoreConfiguration
            {
                ShowFps = true,
                ShowFrameTime = true,
                ShowTotalFrames = true,
                ShowTotalTime = true,
                ShowRootEntitiesCount = true,
                ShowAllEntitiesCount = true,
                ShowGameLoopStatistics = true
            };

            return GetCoreDiagnosticInfoProvider(configuration);
        }

        private static IEnumerable<GetDiagnosticInfoTestCase> GetDiagnosticInfoTestCases()
        {
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return no DiagnosticInfo when configuration has all disabled.",
                CoreConfiguration = new CoreConfiguration(),
                ExpectedNames = Enumerable.Empty<string>().ToArray()
            };
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return FPS when configuration has ShowFps enabled.",
                CoreConfiguration = new CoreConfiguration { ShowFps = true },
                ExpectedNames = new[] { "FPS" }
            };
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return FrameTime when configuration has ShowFrameTime enabled.",
                CoreConfiguration = new CoreConfiguration { ShowFrameTime = true },
                ExpectedNames = new[] { "FrameTime" }
            };
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return TotalFrames when configuration has ShowTotalFrames enabled.",
                CoreConfiguration = new CoreConfiguration { ShowTotalFrames = true },
                ExpectedNames = new[] { "TotalFrames" }
            };
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return TotalTime when configuration has ShowTotalTime enabled.",
                CoreConfiguration = new CoreConfiguration { ShowTotalTime = true },
                ExpectedNames = new[] { "TotalTime" }
            };
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return RootEntitiesCount when configuration has ShowRootEntitiesCount enabled.",
                CoreConfiguration = new CoreConfiguration { ShowRootEntitiesCount = true },
                ExpectedNames = new[] { "RootEntitiesCount" }
            };
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return AllEntitiesCount when configuration has ShowAllEntitiesCount enabled.",
                CoreConfiguration = new CoreConfiguration { ShowAllEntitiesCount = true },
                ExpectedNames = new[] { "AllEntitiesCount" }
            };
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return game loop statistics when configuration has ShowGameLoopStatistics enabled.",
                CoreConfiguration = new CoreConfiguration { ShowGameLoopStatistics = true },
                ExpectedNames = new[] { StepName1, StepName2, StepName3 }
            };
            yield return new GetDiagnosticInfoTestCase
            {
                Description = "Should return all DiagnosticInfo when configuration has all enabled.",
                CoreConfiguration = new CoreConfiguration
                {
                    ShowFps = true,
                    ShowFrameTime = true,
                    ShowTotalFrames = true,
                    ShowTotalTime = true,
                    ShowRootEntitiesCount = true,
                    ShowAllEntitiesCount = true,
                    ShowGameLoopStatistics = true
                },
                ExpectedNames = new[]
                    { "FPS", "FrameTime", "TotalFrames", "TotalTime", "RootEntitiesCount", "AllEntitiesCount", StepName1, StepName2, StepName3 }
            };
        }

        [TestCaseSource(nameof(GetDiagnosticInfoTestCases))]
        public void GetDiagnosticInfo_ReturnsDiagnosticsBasedOnConfiguration(GetDiagnosticInfoTestCase testCase)
        {
            // Arrange
            Debug.Assert(testCase.CoreConfiguration != null, "testCase.CoreConfiguration != null");
            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProvider(testCase.CoreConfiguration);

            // Act
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo().ToList();

            // Assert
            Debug.Assert(testCase.ExpectedNames != null, "testCase.ExpectedNames != null");
            Assert.That(actual.Select(di => di.Name), Is.EquivalentTo(testCase.ExpectedNames));
        }

        [Test]
        public void GetDiagnosticInfo_FPS_ShouldHaveValueOfAvgFpsFromPerformanceStatisticsProvider()
        {
            // Arrange
            const double avgFps = 123.456;
            _performanceStatisticsProvider.AvgFps.Returns(avgFps);
            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled();

            // Act
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo().Single(di => di.Name == "FPS");

            // Assert
            Assert.That(actual.Value, Is.EqualTo(avgFps.ToString(CultureInfo.InvariantCulture)));
        }

        [Test]
        public void GetDiagnosticInfo_FrameTime_ShouldHaveValueOfFrameTimeFromPerformanceStatisticsProvider()
        {
            // Arrange
            var frameTime = TimeSpan.FromMilliseconds(123.456);
            _performanceStatisticsProvider.FrameTime.Returns(frameTime);
            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled();

            // Act
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo().Single(di => di.Name == "FrameTime");

            // Assert
            Assert.That(actual.Value, Is.EqualTo(frameTime.ToString()));
        }

        [Test]
        public void GetDiagnosticInfo_TotalFrames_ShouldHaveValueOfTotalFramesFromPerformanceStatisticsProvider()
        {
            // Arrange
            const int totalFrames = 123;
            _performanceStatisticsProvider.TotalFrames.Returns(totalFrames);
            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled();

            // Act
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo().Single(di => di.Name == "TotalFrames");

            // Assert
            Assert.That(actual.Value, Is.EqualTo(totalFrames.ToString()));
        }

        [Test]
        public void GetDiagnosticInfo_TotalTime_ShouldHaveValueOfTotalTimeFromPerformanceStatisticsProvider()
        {
            // Arrange
            var totalTime = TimeSpan.FromMilliseconds(123.456);
            _performanceStatisticsProvider.TotalTime.Returns(totalTime);
            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled();

            // Act
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo().Single(di => di.Name == "TotalTime");

            // Assert
            Assert.That(actual.Value, Is.EqualTo(totalTime.ToString()));
        }

        [Test]
        public void UpdateDiagnostics_ShouldCauseGetDiagnosticInfoReturn_RootEntitiesCount_Of3_And_AllEntitiesCount_Of5()
        {
            // Arrange
            var configuration = new CoreConfiguration
            {
                ShowRootEntitiesCount = true,
                ShowAllEntitiesCount = true
            };

            var scene = TestSceneFactory.Create();
            var entity1 = scene.CreateEntity();
            _ = entity1.CreateChildEntity();
            var entity2 = scene.CreateEntity();
            _ = entity2.CreateChildEntity();
            _ = scene.CreateEntity();

            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProvider(configuration);

            // Act
            coreDiagnosticInfoProvider.UpdateDiagnostics(scene);

            // Assert
            var diagnosticInfo = coreDiagnosticInfoProvider.GetDiagnosticInfo().ToList();
            var rootEntitiesCount = diagnosticInfo.Single(di => di.Name == "RootEntitiesCount").Value;
            var allEntitiesCount = diagnosticInfo.Single(di => di.Name == "AllEntitiesCount").Value;

            Assert.That(rootEntitiesCount, Is.EqualTo(3.ToString()));
            Assert.That(allEntitiesCount, Is.EqualTo(5.ToString()));
        }

        [Test]
        public void
            GetDiagnosticInfo_GameLoopStatistics_ShouldReturnDiagnosticInfoForEachStepWithAvgFrameTimeAndAvgFrameTimeShareFromPerformanceStatisticsProvider()
        {
            // Arrange
            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled();

            // Act
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo().ToArray();

            // Assert
            var diagnosticInfo1 = actual.Single(di => di.Name == StepName1);
            var diagnosticInfo2 = actual.Single(di => di.Name == StepName2);
            var diagnosticInfo3 = actual.Single(di => di.Name == StepName3);

            Assert.That(diagnosticInfo1.Value, Is.EqualTo($"{_stepStatistics1.AvgFrameTime} [10%]"));
            Assert.That(diagnosticInfo2.Value, Is.EqualTo($"{_stepStatistics2.AvgFrameTime} [20%]"));
            Assert.That(diagnosticInfo3.Value, Is.EqualTo($"{_stepStatistics3.AvgFrameTime} [30%]"));
        }

        public sealed class GetDiagnosticInfoTestCase
        {
            public string? Description { get; set; }
            public CoreConfiguration? CoreConfiguration { get; set; }
            public IEnumerable<string>? ExpectedNames { get; set; }

            public override string ToString()
            {
                return Description ?? string.Empty;
            }
        }
    }
}