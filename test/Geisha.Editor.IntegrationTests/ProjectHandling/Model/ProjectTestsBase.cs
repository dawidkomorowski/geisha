using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Common.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    public class ProjectTestsBase
    {
        private const string TestDirectory = "ProjectTestsDirectory";
        protected static string GetProjectLocation() => Path.Combine(Utils.GetPathUnderTestDirectory(TestDirectory));

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(Utils.GetPathUnderTestDirectory(TestDirectory));
        }

        [TearDown]
        public void TearDown()
        {
            DirectoryRemover.RemoveDirectoryRecursively(Utils.GetPathUnderTestDirectory(TestDirectory));
        }
    }
}