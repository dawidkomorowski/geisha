using System;
using System.Linq;
using NUnit.Framework;

namespace Geisha.Engine.E2ETests
{
    internal static class E2EAssert
    {
        public static void Reported(string appOutput, string assertId, string assertName)
        {
            var lines = appOutput.Split(Environment.NewLine);

            var assertLines = lines.Where(l => l.Contains($"AssertId: {{{assertId}}}")).ToArray();

            switch (assertLines.Length)
            {
                case > 1:
                    Assert.Fail($"AssertId is duplicated: {assertId}");
                    break;
                case < 1:
                    Assert.Fail($"AssertId not found: {assertId}");
                    break;
            }

            Assert.That(assertLines[0], Contains.Substring($"AssertId: {{{assertId}}} Name: {{{assertName}}}"), "Assert not matched.");
        }
    }
}