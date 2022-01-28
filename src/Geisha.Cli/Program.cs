using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            createCommand.AddCommand(CreateCommand_Asset_Create_SpriteAnimation());
            createCommand.AddCommand(CreateCommand_Asset_Create_Texture());

            assetCommand.AddCommand(createCommand);

            return assetCommand;
        }

        private static Command CreateCommand_Asset_Create_InputMapping()
        {
            var command = new Command("input-mapping", "Create default input mapping asset file.");
            command.AddOption(CreateOption_KeepAssetId());
            command.Handler = CommandHandler.Create<bool, IConsole>((keepAssetId, console) =>
            {
                console.Out.WriteLine("Creating input mapping asset file.");
                var createdFile = AssetTool.CreateInputMappingAsset(keepAssetId);
                console.Out.WriteLine($"Input mapping asset file created: {createdFile}");
            });

            return command;
        }

        private static Command CreateCommand_Asset_Create_Sound()
        {
            var command = new Command("sound", "Create sound asset file.");
            var fileArgument = new Argument("file")
            {
                Description = "Path to sound file."
            };
            command.AddArgument(fileArgument);
            command.AddOption(CreateOption_KeepAssetId());
            command.Handler = CommandHandler.Create<FileInfo, bool, IConsole>((file, keepAssetId, console) =>
            {
                console.Out.WriteLine($"Creating sound asset file for: {file.FullName}");
                var createdFile = AssetTool.CreateSoundAsset(file.FullName, keepAssetId);
                console.Out.WriteLine($"Sound asset file created: {createdFile}");
            });

            return command;
        }

        private static Command CreateCommand_Asset_Create_Sprite()
        {
            var command = new Command("sprite", "Create sprite asset file.");
            var fileArgument = new Argument("file")
            {
                Description = "Path to texture asset file or texture file."
            };
            command.AddArgument(fileArgument);
            command.AddOption(CreateOption_KeepAssetId());
            command.AddOption(new Option<double>("-x", "X component of upper left corner of sprite's rectangular region of texture. Default value is 0."));
            command.AddOption(new Option<double>("-y", "Y component of upper left corner of sprite's rectangular region of texture. Default value is 0."));
            command.AddOption(new Option<double>(new[] { "--width", "-w" },
                "Width of sprite's rectangular region of texture. Default value is width of the texture."));
            command.AddOption(new Option<double>(new[] { "--height", "-h" },
                "Height of sprite's rectangular region of texture. Default value is height of the texture."));
            command.AddOption(new Option<int>("--count", () => 1,
                "Number of sprites to create. Texture is treated as sprite sheet with following sprites organized from left to right and from top to bottom. Subsequent sprites move to the right in texture coordinates and then move to the next row. Default value is 1."));
            command.Handler = CommandHandler.Create<FileInfo, bool, double, double, double, double, int, IConsole>(
                (file, keepAssetId, x, y, width, height, count, console) =>
                {
                    console.Out.WriteLine($"Creating sprite asset file for: {file.FullName}");
                    var (spriteAssetFilePaths, textureAssetFilePath) = AssetTool.CreateSpriteAsset(file.FullName, keepAssetId, x, y, width, height, count);

                    if (textureAssetFilePath != null)
                    {
                        console.Out.WriteLine($"Texture asset file created: {textureAssetFilePath}");
                    }

                    foreach (var spriteAssetFilePath in spriteAssetFilePaths)
                    {
                        console.Out.WriteLine($"Sprite asset file created: {spriteAssetFilePath}");
                    }
                });

            return command;
        }

        private static Command CreateCommand_Asset_Create_SpriteAnimation()
        {
            var command = new Command("sprite-animation", "Create sprite animation asset file.");
            var directoryArgument = new Argument("directory")
            {
                Description = "Path to directory containing sprite assets to use for animation. Asset files are composed into animation in alphabetical order."
            };
            command.AddArgument(directoryArgument);
            command.AddOption(new Option<string>("--file-pattern", "Filter sprite asset files to be included in animation using wildcard based pattern."));
            command.AddOption(new Option<FileInfo[]>("--files",
                "Use specified ordered list of sprite asset files. Paths are relative to current working directory."));
            command.AddOption(CreateOption_KeepAssetId());
            command.Handler = CommandHandler.Create<DirectoryInfo, string, FileInfo[], bool, IConsole>(
                (directory, filePattern, files, keepAssetId, console) =>
                {
                    console.Out.WriteLine($"Creating sprite animation asset file for: {directory.FullName}");
                    var spriteAnimationAssetFilePath =
                        AssetTool.CreateSpriteAnimationAsset(
                            directory.FullName,
                            string.IsNullOrEmpty(filePattern) ? null : filePattern,
                            files.Length == 0 ? null : files.Select(f => f.FullName),
                            keepAssetId
                        );
                    console.Out.WriteLine($"Sprite animation asset file created: {spriteAnimationAssetFilePath}");
                });

            return command;
        }

        private static Command CreateCommand_Asset_Create_Texture()
        {
            var command = new Command("texture", "Create texture asset file.");
            var fileArgument = new Argument("file")
            {
                Description = "Path to texture file."
            };
            command.AddArgument(fileArgument);
            command.AddOption(CreateOption_KeepAssetId());
            command.Handler = CommandHandler.Create<FileInfo, bool, IConsole>((file, keepAssetId, console) =>
            {
                console.Out.WriteLine($"Creating texture asset file for: {file.FullName}");
                var createdFile = AssetTool.CreateTextureAsset(file.FullName, keepAssetId);
                console.Out.WriteLine($"Texture asset file created: {createdFile}");
            });

            return command;
        }

        private static Option CreateOption_KeepAssetId()
        {
            return new Option<bool>("--keep-asset-id", "Keep asset id of existing asset file when it is recreated.");
        }
    }
}