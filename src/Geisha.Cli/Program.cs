using System.CommandLine;
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

            return assetCommand;
        }
    }
}