namespace AlfredPlayer
{
    public abstract class APlayer : IPlayer
    {
        public Player wrapper;
        public int status;

        public abstract void Play(string file);
        public abstract void Pause();
        public abstract void PlayPause();
        public abstract void Stop();
        public abstract void SetPosition(double position);
        public abstract void SetVolume(float volume);

        protected APlayer()
        {
            wrapper = Init.form;
        }
    }
}
