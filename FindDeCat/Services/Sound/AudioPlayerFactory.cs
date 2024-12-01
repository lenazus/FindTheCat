namespace FindDeCat.Services.Sound
{
    public static class AudioPlayerFactory
    {
        public static IAudioPlayer CreateAudioPlayer()
        {
#if ANDROID
            return new AudioPlayerForAndroid();
#elif WINDOWS
        return new AudioPlayerForWindows();
#else
        throw new NotImplementedException("No audio player implementation for this platform.");
#endif
        }
    }
}