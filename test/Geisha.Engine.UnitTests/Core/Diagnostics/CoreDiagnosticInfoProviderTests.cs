using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
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

        private IConfigurationManager _configurationManager;
        private IPerformanceStatisticsProvider _performanceStatisticsProvider;
        private SystemExecutionTime _systemExecutionTime1;
        private SystemExecutionTime _systemExecutionTime2;
        private SystemExecutionTime _systemExecutionTime3;

        [SetUp]
        public void SetUp()
        {
            _configurationManager = Substitute.For<IConfigurationManager>();
            _performanceStatisticsProvider = Substitute.For<IPerformanceStatisticsProvider>();

            _systemExecutionTime1 = new SystemExecutionTime(SystemName1, TimeSpan.FromMilliseconds(8), 0.1);
            _systemExecutionTime2 = new SystemExecutionTime(SystemName2, TimeSpan.FromMilliseconds(16), 0.2);
            _systemExecutionTime3 = new SystemExecutionTime(SystemName3, TimeSpan.FromMilliseconds(33), 0.3);

            _performanceStatisticsProvider.GetSystemsExecutionTime().Returns(new[]
            {
                _systemExecutionTime1, _systemExecutionTime2, _systemExecutionTime3
            });
        }

        private CoreDiagnosticInfoProvider GetCoreDiagnosticInfoProvider()
        {
            return new CoreDiagnosticInfoProvider(_configurationManager, _performanceStatisticsProvider);
        }

        private CoreDiagnosticInfoProvider GetCoreDiagnosticInfoProviderWithAllDiagnosticsEnabled()
        {
            var configuration = GetDefaultConfiguration();

            configuration.ShowFps = true;
            configuration.ShowFrameTime = true;
            configuration.ShowTotalFrames = true;
            configuration.ShowTotalTime = true;
            configuration.ShowRootEntitiesCount = true;
            configuration.ShowAllEntitiesCount = true;
            configuration.ShowSystemsExecutionTimes = true;

            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(configuration);
            return GetCoreDiagnosticInfoProvider();
        }

        private static CoreConfiguration GetDefaultConfiguration()
        {
            return new CoreConfiguration();
        }

        private static IEnumerable<GetDiagnosticInfoTestCase> GetDiagnosticInfoTestCases()
        {
            var testCasesData = new[]
            {
                new
                {
                    Description = "Should return no DiagnosticInfo when configuration has all disabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => { }),
                    ExpectedNames = Enumerable.Empty<string>().ToArray()
                },
                new
                {
                    Description = "Should return FPS when configuration has ShowFps enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => configuration.ShowFps = true),
                    ExpectedNames = new[] {"FPS"}
                },
                new
                {
                    Description = "Should return FrameTime when configuration has ShowFrameTime enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => configuration.ShowFrameTime = true),
                    ExpectedNames = new[] {"FrameTime"}
                },
                new
                {
                    Description = "Should return TotalFrames when configuration has ShowTotalFrames enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => configuration.ShowTotalFrames = true),
                    ExpectedNames = new[] {"TotalFrames"}
                },
                new
                {
                    Description = "Should return TotalTime when configuration has ShowTotalTime enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => configuration.ShowTotalTime = true),
                    ExpectedNames = new[] {"TotalTime"}
                },
                new
                {
                    Description = "Should return RootEntitiesCount when configuration has ShowRootEntitiesCount enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => configuration.ShowRootEntitiesCount = true),
                    ExpectedNames = new[] {"RootEntitiesCount"}
                },
                new
                {
                    Description = "Should return AllEntitiesCount when configuration has ShowAllEntitiesCount enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => configuration.ShowAllEntitiesCount = true),
                    ExpectedNames = new[] {"AllEntitiesCount"}
                },
                new
                {
                    Description = "Should return AllEntitiesCount when configuration has ShowAllEntitiesCount enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => configuration.ShowAllEntitiesCount = true),
                    ExpectedNames = new[] {"AllEntitiesCount"}
                },
                new
                {
                    Description = "Should return system execution time for each system when configuration has ShowSystemsExecutionTimes enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration => configuration.ShowSystemsExecutionTimes = true),
                    ExpectedNames = new[] {SystemName1, SystemName2, SystemName3}
                },
                new
                {
                    Description = "Should return all DiagnosticInfo when configuration has all enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration =>
                    {
                        configuration.ShowFps = true;
                        configuration.ShowFrameTime = true;
                        configuration.ShowTotalFrames = true;
                        configuration.ShowTotalTime = true;
                        configuration.ShowRootEntitiesCount = true;
                        configuration.ShowAllEntitiesCount = true;
                        configuration.ShowSystemsExecutionTimes = true;
                    }),
                    ExpectedNames = new[]
                        {"FPS", "FrameTime", "TotalFrames", "TotalTime", "RootEntitiesCount", "AllEntitiesCount", SystemName1, SystemName2, SystemName3}
                }
            };

            foreach (var testCaseData in testCasesData)
            {
                var coreConfiguration = GetDefaultConfiguration();
                testCaseData.PrepareAction(coreConfiguration);

                yield return new GetDiagnosticInfoTestCase
                {
                    Description = testCaseData.Description,
                    CoreConfiguration = coreConfiguration,
                    ExpectedNames = testCaseData.ExpectedNames
                };
            }
        }

        [TestCaseSource(nameof(GetDiagnosticInfoTestCases))]
        public void GetDiagnosticInfo_ReturnsDiagnosticsBasedOnConfiguration(GetDiagnosticInfoTestCase testCase)
        {
            // Arrange
            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(testCase.CoreConfiguration);
            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProvider();

            // Act
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo().ToList();

            // Assert
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
            var coreConfiguration = GetDefaultConfiguration();
            coreConfiguration.ShowRootEntitiesCount = true;
            coreConfiguration.ShowAllEntitiesCount = true;

            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(coreConfiguration);

            var entity1 = new Entity();
            entity1.AddChild(new Entity());
            var entity2 = new Entity();
            entity2.AddChild(new Entity());
            var entity3 = new Entity();

            var scene = new Scene();
            scene.AddEntity(entity1);
            scene.AddEntity(entity2);
            scene.AddEntity(entity3);

            var coreDiagnosticInfoProvider = GetCoreDiagnosticInfoProvider();

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
            var actual = coreDiagnosticInfoProvider.GetDiagnosticInfo();

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
            public string Description { get; set; }
            public CoreConfiguration CoreConfiguration { get; set; }
            public IEnumerable<string> ExpectedNames { get; set; }

            public override string ToString()
            {
                return Description;
            }
        }
    }
}