using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Geisha.Cli
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Description = "Geisha CLI provides set of development tools for Geisha Engine."
            };

            rootCommand.AddCommand(CreateAssetCommand());

            return await rootCommand.InvokeAsync(args);
        }

        private static Command CreateAssetCommand()
        {
            var assetCommand = new Command("asset", "Create asset files.");

            var createCommand = new Command("create", "Create asset files.");

            var soundCommand = new Command("sound", "Create sound asset file.");
            var fileArgument = new Argument("file")
            {
                Description = "Path to sound file."
            };
            soundCommand.AddArgument(fileArgument);
            soundCommand.Handler = CommandHandler.Create<FileInfo>(file => { Console.WriteLine($"Creating sound asset file for: {file.Name}"); });

            createCommand.AddCommand(soundCommand);

            assetCommand.AddCommand(createCommand);

            return assetCommand;
        }
    }
}