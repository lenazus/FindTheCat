#if WINDOWS
using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace FindDeCat.Services.Sound
{
    public class AudioPlayerForWindows : IAudioPlayer
    {
        public async Task<(bool Success, string ErrorMessage)> PlaySoundAsync(string soundFilePath)
        {
            try
            {
                var basePath = AppContext.BaseDirectory;
                var newSoundPath = Path.Combine(basePath, "Resources", "Sounds", soundFilePath);

                // Ensure the file exists
                if (!File.Exists(newSoundPath))
                {
                    return (false, $"The sound file {soundFilePath} could not be found.");
                }

                using (var audioFile = new AudioFileReader(newSoundPath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    // Wait until the sound is finished playing
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100); // Check every 100ms
                    }
                }

                return (true, "OK"); // Success
            }
            catch (Exception ex)
            {
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
#endif
