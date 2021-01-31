namespace Game.Helpers.Audio
{
    public interface IAudioManager
    {
        IAudioPlaybackInstance Play(int clip, bool loops = false);
        void StopAndClearAll();
    }

    public interface IAudioPlaybackInstance
    {
        void Stop();
        float Volume { get; set; }
        bool Looping { get; set; }
        bool IsPlaying { get; }
        float Pitch { get; set; }
    }
}