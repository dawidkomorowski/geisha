using System.IO;
using Geisha.Engine.Core.Math;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Geisha.TestUtils;

public static class Utils
{
    public static string TestDirectory => TestContext.CurrentContext.TestDirectory;
    public static string GetPathUnderTestDirectory(string path) => Path.Combine(TestDirectory, path);
    public static Randomizer Random => TestContext.CurrentContext.Random;
    public static Vector2 RandomVector2() => new(Random.NextDouble(), Random.NextDouble());
    public static Vector3 RandomVector3() => new(Random.NextDouble(), Random.NextDouble(), Random.NextDouble());
}