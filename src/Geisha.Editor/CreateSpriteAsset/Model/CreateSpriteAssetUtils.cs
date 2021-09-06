﻿using System.IO;
using Geisha.Editor.CreateTextureAsset.Model;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;

namespace Geisha.Editor.CreateSpriteAsset.Model
{
    internal static class CreateSpriteAssetUtils
    {
        public static bool CanCreateSpriteAssetFromFile(string filePath)
        {
            return IsTextureAssetFile(filePath) || IsTextureFile(filePath);
        }

        public static bool IsTextureAssetFile(string filePath)
        {
            if (!AssetFileUtils.IsAssetFile(filePath)) return false;
            return AssetData.Load(filePath).AssetType == RenderingAssetTypes.Texture;
        }

        public static bool IsTextureFile(string filePath)
        {
            return TextureFileFormat.IsSupported(Path.GetExtension(filePath));
        }
    }
}