﻿using System.ComponentModel.Composition.Hosting;
using System.IO;
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

        protected string GetRandomFilePath() => Path.Combine(TestContext.CurrentContext.TestDirectory, Path.GetRandomFileName());
        protected Randomizer Random => TestContext.CurrentContext.Random;
    }
}