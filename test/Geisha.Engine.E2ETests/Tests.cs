using NUnit.Framework;
using System.Diagnostics;
using System;

namespace Geisha.Engine.E2ETests
{
    [Timeout(5000)]
    public class Tests
    {
        [Test]
        public void EngineApiCanBeInjectedToCustomGameCode()
        {
            var output = RunE2EApp();

            Assert.That(output, Contains.Substring("484E1AFA-EEFE-4E3A-9D8E-A304847C8C16 EngineApiDependencyInjectionTestComponent"));
        }

        private static string RunE2EApp()
        {
            var processStartInfo = new ProcessStartInfo("Geisha.Engine.E2EApp.exe")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var app = Process.Start(processStartInfo) ?? throw new InvalidOperationException("Process could not be started.");
            var standardError = app.StandardError.ReadToEnd();
            var standardOutput = app.StandardOutput.ReadToEnd();

            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Geisha.Engine.E2EApp.exe output start");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine(standardError);
            Console.WriteLine(standardOutput);
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Geisha.Engine.E2EApp.exe output end");
            Console.WriteLine("-------------------------------------");

            if (standardError != string.Empty)
            {
                Assert.Fail(standardError);
            }

            return standardOutput;
        }
    }
}