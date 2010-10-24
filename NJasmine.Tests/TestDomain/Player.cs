namespace NJasmine.Tests.TestDomain
{
    public class Player
    {
        public void Play(Song song)
        {
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }

        public Song CurrentSong { get; private set; }
        public bool IsPlaying { get; private set; }
    }
}