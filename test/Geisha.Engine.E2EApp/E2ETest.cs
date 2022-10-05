using System;

namespace Geisha.Engine.E2EApp
{
    internal static class E2ETest
    {
        public static void Report(string assertId, string assertName)
        {
            Console.WriteLine($"AssertId: {{{assertId}}} Name: {{{assertName}}}");
        }
    }
}