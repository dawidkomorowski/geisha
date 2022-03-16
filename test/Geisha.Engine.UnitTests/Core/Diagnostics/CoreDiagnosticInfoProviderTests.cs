using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
{
    [TestFixture]
    public class CoreDiagnosticInfoProviderTests
    {
        private const string SystemName1 = "System 1";
        private const string SystemName2 = "System 2";
        private const string SystemName3 = "System 3";

        private IPerformanceStatisticsProvider _performanceStatisticsProvider = null!;
        private SystemExecutionTime _systemExecutionTime1 = null!;
        private SystemExecutionTime _systemExecutionTime2 = null!;
        private SystemExecutionTime _systemExecutionTime3 = null!;

        [SetUp]
        public void SetUp()
        {
            _performanceStatisticsProvider = Substitute.For<IPerformanceStatisticsProvider>();

            _systemExecutionTime1 = new SystemExecutionTime(SystemName1, TimeSpan.FromMilliseconds(8), 0.1);
            _systemExecutionTime2 = new SystemExecutionTime(SystemName2, TimeSpan.FromMilliseconds(16), 0.2);
            _systemExecutionTime3 = new SystemExecutionTime(SystemName3, TimeSpan.FromMilliseconds(33), 0.3);

            _performanceStatisticsProvider.GetSystemsExecutionTime().Returns(new[]
            {
                _systemExecutionTime1, _systemExecutionTime2, _systemExecutionTime3
            });
        }

        private CoreDiagnosticInfoProvider GetCoreDiagnosticInfoProvider(CoreConfiguration configuration)
        {
            return new CoreDiagnosticInfoProvider(configuration, _performanceStatisticsProvider);
        }

        private CoreDiagnosticInfoProvider GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled()
        {
            var configurationBuilder = CoreConfiguration.CreateBuilder();

            configurationBuilder.WithShowFps(true);
            configurationBuilder.WithShowFrameTime(true);
            configurationBuilder.WithShowTotalFrames(true);
            configurationBuilder.WithShowTotalTime(true);
            configurationBuilder.WithShowRootEntitiesCount(true);
            configurationBuilder.WithShowAllEntitiesCount(true);
            configurationBuilder.WithShowSystemsExecutionTimes(true);

            return GetCoreDiagnosticInfoProvider(configurationBuilder.Build());
        }

        private static IEnumerable<GetDiagnosticInfoTestCase> GetDiagnosticInfoTestCases()
        {
            var testCasesData = new[]
            {
                new
                {
                    Description = "Should return no DiagnosticInfo when configuration has all disabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder => { }),
                    ExpectedNames = Enumerable.Empty<string>().ToArray()
                },
                new
                {
                    Description = "Should return FPS when configuration has ShowFps enabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder => builder.WithShowFps(true)),
                    ExpectedNames = new[] { "FPS" }
                },
                new
                {
                    Description = "Should return FrameTime when configuration has ShowFrameTime enabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder => builder.WithShowFrameTime(true)),
                    ExpectedNames = new[] { "FrameTime" }
                },
                new
                {
                    Description = "Should return TotalFrames when configuration has ShowTotalFrames enabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder => builder.WithShowTotalFrames(true)),
                    ExpectedNames = new[] { "TotalFrames" }
                },
                new
                {
                    Description = "Should return TotalTime when configuration has ShowTotalTime enabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder => builder.WithShowTotalTime(true)),
                    ExpectedNames = new[] { "TotalTime" }
                },
                new
                {
                    Description = "Should return RootEntitiesCount when configuration has ShowRootEntitiesCount enabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder => builder.WithShowRootEntitiesCount(true)),
                    ExpectedNames = new[] { "RootEntitiesCount" }
                },
                new
                {
                    Description = "Should return AllEntitiesCount when configuration has ShowAllEntitiesCount enabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder => builder.WithShowAllEntitiesCount(true)),
                    ExpectedNames = new[] { "AllEntitiesCount" }
                },
                new
                {
                    Description = "Should return system execution time for each system when configuration has ShowSystemsExecutionTimes enabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder => builder.WithShowSystemsExecutionTimes(true)),
                    ExpectedNames = new[] { SystemName1, SystemName2, SystemName3 }
                },
                new
                {
                    Description = "Should return all DiagnosticInfo when configuration has all enabled.",
                    PrepareAction = new Action<CoreConfiguration.IBuilder>(builder =>
                    {
                        builder.WithShowFps(true);
                        builder.WithShowFrameTime(true);
                        builder.WithShowTotalFrames(true);
                        builder.WithShowTotalTime(true);
                        builder.WithShowRootEntitiesCount(true);
                        builder.WithShowAllEntitiesCount(true);
                        builder.WithShowSystemsExecutionTimes(true);
                    }),
                    ExpectedNames = new[]
                        { "FPS", "FrameTime", "TotalFrames", "TotalTime", "RootEntitiesCount", "AllEntitiesCount", SystemName1, SystemName2, SystemName3 }
                }
            };

            foreach (var testCaseData in testCasesData)
            {
                var configurationBuilder = CoreConfiguration.CreateBuilder();
                testCaseData.PrepareAction(configurationBuilder);

                yield return new GetDiagnosticInfoTestCase
                {
                    Description = testCaseData.Description,
                    CoreConfiguration = configurationBuilder.Build(),
                    ExpectedNames = testCaseData.ExpectedNames
                };
            }
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
            var configurationBuilder = CoreConfiguration.CreateBuilder();
            configurationBuilder.WithShowRootEntitiesCount(true);
            configurationBuilder.WithShowAllEntitiesCount(true);

            var scene = TestSceneFactory.Create();
            var entity1 = scene.CreateEntity();
            _ = entity1.CreateChildEntity();
            var entity2 = scene.CreateEntity();
            _ = entity2.CreateChildEntity();
            _ = scene.CreateEntity();

            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProvider(configurationBuilder.Build());

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
            GetDiagnosticInfo_SystemsExecutionTimes_ShouldReturnDiagnosticInfoForEachSystemWithAvgFrameTimeAndAvgFrameTimeShareFromPerformanceStatisticsProvider()
        {
            // Arrange
            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled();

            // Act
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo().ToArray();

            // Assert
            var diagnosticInfo1 = actual.Single(di => di.Name == SystemName1);
            var diagnosticInfo2 = actual.Single(di => di.Name == SystemName2);
            var diagnosticInfo3 = actual.Single(di => di.Name == SystemName3);

            Assert.That(diagnosticInfo1.Value, Is.EqualTo($"{_systemExecutionTime1.AvgFrameTime} [10%]"));
            Assert.That(diagnosticInfo2.Value, Is.EqualTo($"{_systemExecutionTime2.AvgFrameTime} [20%]"));
            Assert.That(diagnosticInfo3.Value, Is.EqualTo($"{_systemExecutionTime3.AvgFrameTime} [30%]"));
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