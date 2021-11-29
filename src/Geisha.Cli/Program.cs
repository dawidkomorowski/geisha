using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO;
using System.Threading.Tasks;
using Geisha.Tools;

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

            rootCommand.AddCommand(CreateCommand_Asset());

            return await rootCommand.InvokeAsync(args);
        }

        private static Command CreateCommand_Asset()
        {
            var assetCommand = new Command("asset", "Create asset files.");

            var createCommand = new Command("create", "Create asset files.");
            createCommand.AddCommand(CreateCommand_Asset_Create_InputMapping());
            createCommand.AddCommand(CreateCommand_Asset_Create_Sound());
            createCommand.AddCommand(CreateCommand_Asset_Create_Sprite());
            createCommand.AddCommand(CreateCommand_Asset_Create_Texture());

            assetCommand.AddCommand(createCommand);

            return assetCommand;
        }

        private static Command CreateCommand_Asset_Create_InputMapping()
        {
            var inputMappingCommand = new Command("input-mapping", "Create default input mapping asset file.");
            inputMappingCommand.Handler = CommandHandler.Create<IConsole>(console =>
            {
                console.Out.WriteLine("Creating input mapping asset file.");
                var createdFile = AssetTool.CreateInputMappingAsset();
                console.Out.WriteLine($"Input mapping asset file created: {createdFile}");
            });

            return inputMappingCommand;
        }

        private static Command CreateCommand_Asset_Create_Sound()
        {
            var soundCommand = new Command("sound", "Create sound asset file.");
            var fileArgument = new Argument("file")
            {
                Description = "Path to sound file."
            };
            soundCommand.AddArgument(fileArgument);
            soundCommand.Handler = CommandHandler.Create<FileInfo, IConsole>((file, console) =>
            {
                console.Out.WriteLine($"Creating sound asset file for: {file.FullName}");
                var createdFile = AssetTool.CreateSoundAsset(file.FullName);
                console.Out.WriteLine($"Sound asset file created: {createdFile}");
            });

            return soundCommand;
        }

        private static Command CreateCommand_Asset_Create_Sprite()
        {
            var spriteCommand = new Command("sprite", "Create sprite asset file.");
            var fileArgument = new Argument("file")
            {
                Description = "Path to texture asset file or texture file."
            };
            spriteCommand.AddArgument(fileArgument);
            spriteCommand.Handler = CommandHandler.Create<FileInfo, IConsole>((file, console) =>
            {
                console.Out.WriteLine($"Creating sprite asset file for: {file.FullName}");
                var (spriteAssetFilePath, textureAssetFilePath) = AssetTool.CreateSpriteAsset(file.FullName);

                if (textureAssetFilePath != null)
                {
                    console.Out.WriteLine($"Texture asset file created: {textureAssetFilePath}");
                }

                console.Out.WriteLine($"Sprite asset file created: {spriteAssetFilePath}");
            });

            return spriteCommand;
        }

        private static Command CreateCommand_Asset_Create_Texture()
        {
            var textureCommand = new Command("texture", "Create texture asset file.");
            var fileArgument = new Argument("file")
            {
                Description = "Path to texture file."
            };
            textureCommand.AddArgument(fileArgument);
            textureCommand.Handler = CommandHandler.Create<FileInfo, IConsole>((file, console) =>
            {
                console.Out.WriteLine($"Creating texture asset file for: {file.FullName}");
                var createdFile = AssetTool.CreateTextureAsset(file.FullName);
                console.Out.WriteLine($"Texture asset file created: {createdFile}");
            });

            return textureCommand;
        }
    }
}