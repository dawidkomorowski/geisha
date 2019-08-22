using System;

namespace Geisha.Engine.Audio
{
    public static class SoundFormatParser
    {
        public static SoundFormat ParseFromFileExtension(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".wav":
                    return SoundFormat.Wav;
                case ".mp3":
                    return SoundFormat.Mp3;
                default:
                    throw new UnsupportedSoundFileFormatException(fileExtension);
            }
        }
    }

    /// <summary>
    ///     The exception that is thrown when loading an asset that is already loaded by particular managed asset instance.
    /// </summary>
    public sealed class UnsupportedSoundFileFormatException : Exception
    {
        public UnsupportedSoundFileFormatException(string fileExtension) : base(
            $"File extension: {fileExtension} specifies unsupported sound file format. Supported sound file formats are: {SoundFormat.Wav}, {SoundFormat.Mp3}.")
        {
            FileExtension = fileExtension;
        }

        /// <summary>
        ///     Asset info of asset that loading has failed.
        /// </summary>
        public string FileExtension { get; }
    }
}