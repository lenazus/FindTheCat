namespace FindDeCat.Services.Sound
{
    public class SoundPlayerService : ISoundPlayerService
    {
        private readonly IAudioPlayer _audioPlayer;

        public SoundPlayerService()
        {
            _audioPlayer = AudioPlayerFactory.CreateAudioPlayer();
        }

        public async Task<(bool Success, string ErrorMessage)> PlaySoundAsync(string soundFileName)
        {
            try
            {
                var soundFilePath = soundFileName;

                var (result, resultMessage) = await _audioPlayer.PlaySoundAsync(soundFilePath);

                if (!result)
                {
                    return (false, resultMessage);
                }

                return (true, "Sound played successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}