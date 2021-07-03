using System;
using System.IO;

namespace Geisha.Engine.Core.Assets
{
    public sealed class AssetFile
    {
        public static AssetFile Create(AssetId assetId, AssetType assetType, Stream binaryContent)
        {
            throw new NotImplementedException();
        }

        public static AssetFile Create(AssetId assetId, AssetType assetType, string stringContent)
        {
            throw new NotImplementedException();
        }

        public static AssetFile Create<T>(AssetId assetId, AssetType assetType, T jsonContent)
        {
            throw new NotImplementedException();
        }

        public static AssetFile Load(string filePath)
        {
            throw new NotImplementedException();
        }

        public static AssetFile Load(Stream stream)
        {
            throw new NotImplementedException();
        }

        public AssetId AssetId { get; }
        public AssetType AssetType { get; }

        public Stream ReadBinaryContent()
        {
            throw new NotImplementedException();
        }

        public string ReadStringContent()
        {
            throw new NotImplementedException();
        }

        public T ReadJsonContent<T>()
        {
            throw new NotImplementedException();
        }

        public void Save(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        private AssetFile(AssetId assetId, AssetType assetType)
        {
            AssetId = assetId;
            AssetType = assetType;
        }
    }
}