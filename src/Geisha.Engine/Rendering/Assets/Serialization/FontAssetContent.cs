namespace Geisha.Engine.Rendering.Assets.Serialization;

/// <summary>
///     Defines font asset content used to locate the font file associated with a font asset.
/// </summary>
public sealed class FontAssetContent
{
    /// <summary>
    ///     Path to the font file, relative to the font asset file.
    /// </summary>
    public string? FontFilePath { get; set; }
}