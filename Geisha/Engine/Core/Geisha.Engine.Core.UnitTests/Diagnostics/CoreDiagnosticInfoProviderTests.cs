using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Diagnostics
{
    [TestFixture]
    public class CoreDiagnosticInfoProviderTests
    {
        private IConfigurationManager _configurationManager;
        private IPerformanceStatisticsProvider _performanceStatisticsProvider;

        [SetUp]
        public void SetUp()
        {
            _configurationManager = Substitute.For<IConfigurationManager>();
            _performanceStatisticsProvider = Substitute.For<IPerformanceStatisticsProvider>();
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

            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(configuration);
            return GetCoreDiagnosticInfoProvider();
        }

        private static CoreConfiguration GetDefaultConfiguration()
        {
            var coreDefaultConfigurationFactory = new CoreDefaultConfigurationFactory();
            return (CoreConfiguration) coreDefaultConfigurationFactory.CreateDefault();
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
                    Description = "Should return all DiagnosticInfo when configuration has all enabled.",
                    PrepareAction = new Action<CoreConfiguration>(configuration =>
                    {
                        configuration.ShowFps = true;
                        configuration.ShowFrameTime = true;
                        configuration.ShowTotalFrames = true;
                        configuration.ShowTotalTime = true;
                        configuration.ShowRootEntitiesCount = true;
                        configuration.ShowAllEntitiesCount = true;
                    }),
                    ExpectedNames = new[] {"FPS", "FrameTime", "TotalFrames", "TotalTime", "RootEntitiesCount", "AllEntitiesCount"}
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
            Assert.That(actual.Value, Is.EqualTo(avgFps));
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
            Assert.That(actual.Value, Is.EqualTo(frameTime));
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
            Assert.That(actual.Value, Is.EqualTo(totalFrames));
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
            Assert.That(actual.Value, Is.EqualTo(totalTime));
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

            Assert.That(rootEntitiesCount, Is.EqualTo(3));
            Assert.That(allEntitiesCount, Is.EqualTo(5));
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