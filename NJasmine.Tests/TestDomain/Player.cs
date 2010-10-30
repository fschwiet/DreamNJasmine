namespace NJasmine.Tests.TestDomain
{
    public class Player
    {
        Song _lastSong;

        public void Play(Song song)
        {
            CurrentSong = song;
            IsPlaying = true;
        }

        public void Pause()
        {
            _lastSong = CurrentSong;
            CurrentSong = null;
            IsPlaying = false;
        }

        public void Resume()
        {
            CurrentSong = _lastSong;
            IsPlaying = true;
        }

        public Song CurrentSong { get; private set; }
        public bool IsPlaying { get; private set; }
    }
}