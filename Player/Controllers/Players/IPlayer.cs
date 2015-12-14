namespace AlfredPlayer
{
    interface IPlayer
    {
        void Play(string file);
        void Pause();
        void PlayPause();
        void Stop();
        void SetPosition(double position);
        void SetVolume(float volume);
    }
}
