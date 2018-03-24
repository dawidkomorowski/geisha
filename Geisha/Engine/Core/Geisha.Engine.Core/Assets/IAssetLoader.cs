using System;

namespace Geisha.Engine.Core.Assets
{
    public interface IAssetLoader
    {
        Type AssetType { get; }
        object Load(string filePath);
    }
}