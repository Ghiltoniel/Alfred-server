namespace Alfred.Model.Core.Music
{
    public class PlayerStatus
    {
        private int Channel;
        public int Status;
        public int CurrentPlaylistIndex;
        public double Position;
        public double Length;
        public float Volume;
        public bool Synchronized;

        public PlayerStatus(int channel)
        {
            Channel = channel;
            CurrentPlaylistIndex = 0;
            Status = -1;
            Length = 0;
            Position = 0;
            Volume = 0;
            Synchronized = false;
        }
    }
}
