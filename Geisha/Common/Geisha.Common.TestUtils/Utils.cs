using System.IO;
using Geisha.Common.Math;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Geisha.Common.TestUtils
{
    public static class Utils
    {
        public static string TestDirectory => TestContext.CurrentContext.TestDirectory;
        public static string GetPathUnderTestDirectory(string path) => Path.Combine(TestDirectory, path);
        public static string GetRandomFilePath() => GetPathUnderTestDirectory(Path.GetRandomFileName());
        public static Randomizer Random => TestContext.CurrentContext.Random;
        public static Vector2 NewRandomVector2() => new Vector2(Random.NextDouble(), Random.NextDouble());
    }
}
