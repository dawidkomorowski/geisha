namespace Geisha.Engine.Rendering;

/// <summary>
///     Provides the file path of the font associated with a font asset.
/// </summary>
public sealed class FontFileInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FontFileInfo"/> class.
    /// </summary>
    /// <param name="path">The path to the font file.</param>
    public FontFileInfo(string path)
    {
        Path = path;
    }

    /// <summary>
    ///     Gets the path to the font file.
    /// </summary>
    public string Path { get; }
}