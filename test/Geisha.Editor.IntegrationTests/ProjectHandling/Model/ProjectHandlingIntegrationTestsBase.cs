using System;
using System.IO;
using System.Threading;
using Geisha.Common.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    public class ProjectHandlingIntegrationTestsBase
    {
        private string _testDirectory;

        protected string GetProjectLocation() => _testDirectory != null
            ? Path.Combine(Utils.GetPathUnderTestDirectory(_testDirectory))
            : throw new InvalidOperationException($"{nameof(GetProjectLocation)} method is safe to call only after test setup and before test teardown.");

        [SetUp]
        public void SetUp()
        {
            _testDirectory = $"ProjectTestsDirectory_{Path.GetRandomFileName()}";
            Directory.CreateDirectory(Utils.GetPathUnderTestDirectory(_testDirectory));
        }

        [TearDown]
        public void TearDown()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(50)); // Waiting a bit seems to solve instability with error "Directory is not empty".
            Directory.Delete(Utils.GetPathUnderTestDirectory(_testDirectory), true);
            _testDirectory = null;
        }
    }
}