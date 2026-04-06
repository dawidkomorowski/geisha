using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Geisha.Engine.E2ETests;

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

        var errorTask = app.StandardError.ReadToEndAsync();
        var outputTask = app.StandardOutput.ReadToEndAsync();

        if (!app.WaitForExit(30_000))
        {
            app.Kill(true);
            Assert.Fail("E2E app did not exit in time.");
        }

        Task.WaitAll(errorTask, outputTask);

        var standardError = errorTask.Result;
        var standardOutput = outputTask.Result;

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