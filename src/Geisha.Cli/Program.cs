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
            inputMappingCommand.AddOption(CreateOption_KeepAssetId());
            inputMappingCommand.Handler = CommandHandler.Create<bool, IConsole>((keepAssetId, console) =>
            {
                console.Out.WriteLine("Creating input mapping asset file.");
                var createdFile = AssetTool.CreateInputMappingAsset(keepAssetId);
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
            soundCommand.AddOption(CreateOption_KeepAssetId());
            soundCommand.Handler = CommandHandler.Create<FileInfo, bool, IConsole>((file, keepAssetId, console) =>
            {
                console.Out.WriteLine($"Creating sound asset file for: {file.FullName}");
                var createdFile = AssetTool.CreateSoundAsset(file.FullName, keepAssetId);
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
            spriteCommand.AddOption(CreateOption_KeepAssetId());
            spriteCommand.Handler = CommandHandler.Create<FileInfo, bool, IConsole>((file, keepAssetId, console) =>
            {
                console.Out.WriteLine($"Creating sprite asset file for: {file.FullName}");
                var (spriteAssetFilePath, textureAssetFilePath) = AssetTool.CreateSpriteAsset(file.FullName, keepAssetId);

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
            textureCommand.AddOption(CreateOption_KeepAssetId());
            textureCommand.Handler = CommandHandler.Create<FileInfo, bool, IConsole>((file, keepAssetId, console) =>
            {
                console.Out.WriteLine($"Creating texture asset file for: {file.FullName}");
                var createdFile = AssetTool.CreateTextureAsset(file.FullName, keepAssetId);
                console.Out.WriteLine($"Texture asset file created: {createdFile}");
            });

            return textureCommand;
        }

        private static Option CreateOption_KeepAssetId()
        {
            return new Option<bool>("--keep-asset-id", "Keep asset id of existing asset file when it is recreated.");
        }
    }
}