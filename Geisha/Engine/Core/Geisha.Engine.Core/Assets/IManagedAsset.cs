namespace Geisha.Engine.Core.Assets
{
    // TODO Add XML documentation.
    public interface IManagedAsset
    {
        AssetInfo AssetInfo { get; }
        object AssetInstance { get; }
        bool IsLoaded { get; }

        void Load();
        void Unload();
    }
}