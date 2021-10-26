using System;

namespace Geisha.Engine.Audio
{
    /// <summary>
    ///     Provides methods for inspecting and parsing <see cref="SoundFormat" /> from file extension.
    /// </summary>
    public static class SoundFormatParser
    {
        /// <summary>
        ///     Checks if specified file extension is one of supported sound file formats.
        /// </summary>
        /// <param name="fileExtension">File extension to inspect.</param>
        /// <returns><c>true</c> if file extension is one of supported sound file formats; otherwise <c>false</c>.</returns>
        public static bool IsSupportedFileExtension(string fileExtension)
        {
            return fileExtension switch
            {
                ".wav" => true,
                ".mp3" => true,
                _ => false
            };
        }

        /// <summary>
        ///     Parses file extension into value of <see cref="SoundFormat" />.
        /// </summary>
        /// <param name="fileExtension">File extension to parse.</param>
        /// <returns><see cref="SoundFormat" /> value representing parsed file extension.</returns>
        /// <exception cref="UnsupportedSoundFileFormatException">
        ///     Sound file format represented by the file extension is
        ///     unsupported.
        /// </exception>
        public static SoundFormat ParseFromFileExtension(string fileExtension)
        {
            return fileExtension switch
            {
                ".wav" => SoundFormat.Wav,
                ".mp3" => SoundFormat.Mp3,
                _ => throw new UnsupportedSoundFileFormatException(fileExtension)
            };
        }
    }

    /// <summary>
    ///     The exception that is thrown when parsing the file extension into value of <see cref="SoundFormat" />.
    /// </summary>
    public sealed class UnsupportedSoundFileFormatException : Exception
    {
        /// <summary>
        ///     Create new instance of <see cref="UnsupportedSoundFileFormatException" /> with specified
        ///     <paramref name="fileExtension" />.
        /// </summary>
        /// <param name="fileExtension">Unsupported file extension.</param>
        public UnsupportedSoundFileFormatException(string fileExtension) : base(
            $"File extension: {fileExtension} specifies unsupported sound file format. Supported sound file formats are: {SoundFormat.Wav}, {SoundFormat.Mp3}.")
        {
            FileExtension = fileExtension;
        }

        /// <summary>
        ///     Unsupported file extension.
        /// </summary>
        public string FileExtension { get; }
    }
}