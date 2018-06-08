using System.ComponentModel.Composition.Hosting;
using System.IO;
using Geisha.Common.Math;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Geisha.Engine.IntegrationTests
{
    public class IntegrationTests<TSystemUnderTest>
    {
        private CompositionContainer _compositionContainer;
        protected TSystemUnderTest SystemUnderTest { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            _compositionContainer = new CompositionContainer(new ApplicationCatalog());
            SystemUnderTest = _compositionContainer.GetExportedValue<TSystemUnderTest>();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _compositionContainer.Dispose();
        }

        protected string TestDirectory => TestContext.CurrentContext.TestDirectory;
        protected string GetPathUnderTestDirectory(string path) => Path.Combine(TestDirectory, path);
        protected string GetRandomFilePath() => GetPathUnderTestDirectory(Path.GetRandomFileName());
        protected Randomizer Random => TestContext.CurrentContext.Random;
        protected Vector2 NewRandomVector2() => new Vector2(Random.NextDouble(), Random.NextDouble());
    }
}