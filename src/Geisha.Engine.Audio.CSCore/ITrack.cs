namespace Geisha.Engine.Audio.CSCore
{
    internal interface ITrack
    {
        bool IsPlaying { get; }

        void Play();
        void Pause();
        void Stop();
    }
}