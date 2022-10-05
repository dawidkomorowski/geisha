using NUnit.Framework;

namespace Geisha.Engine.E2ETests
{
    internal static class E2EAssert
    {
        public static void Reported(string appOutput, string assertId, string assertName)
        {
            Assert.That(appOutput, Contains.Substring($"AssertId: {{{assertId}}} Name: {{{assertName}}}"));
        }
    }
}