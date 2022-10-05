using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Geisha.Engine.E2ETests
{
    public static class E2EApp
    {
        public static string Run(string startUpSceneBehavior)
        {
            const string engineConfigFilePath = "engine-config.json";
            var engineConfig = File.ReadAllText(engineConfigFilePath);
            engineConfig = Regex.Replace(engineConfig, @"""StartUpSceneBehavior"": ""\w*""", $@"""StartUpSceneBehavior"": ""{startUpSceneBehavior}""");
            File.WriteAllText(engineConfigFilePath, engineConfig);

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