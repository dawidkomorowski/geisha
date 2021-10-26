using System;

namespace Geisha.Engine.Audio
{
    public static class SoundFormatParser
    {
        public static bool IsSupportedFileExtension(string fileExtension)
        {
            return fileExtension switch
            {
                ".wav" => true,
                ".mp3" => true,
                _ => false
            };
        }

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