#if ANDROID
using Android.Media;
using System.Threading.Tasks;
using Android.Content.Res;
using Android.Content;
using System.IO;

namespace FindDeCat.Services.Sound
{
    public class AudioPlayerForAndroid : IAudioPlayer
    {
        public async Task<(bool Success, string ErrorMessage)> PlaySoundAsync(string soundFileName)
        {
            var context = Android.App.Application.Context;
            if (context == null)
            {
                return (false, "AudioPlayerForAndroid: Android.App.Application.Context is null.");
            }

            if (!soundFileName.StartsWith("Resources/Sounds/"))
            {
                soundFileName = $"Resources/Sounds/{soundFileName}";
            }

            try
            {
                AssetManager assetManager = context.Assets;

                using (var assetFileDescriptor = assetManager.OpenFd(soundFileName))
                {
                    MediaPlayer mediaPlayer = new MediaPlayer();
                    mediaPlayer.SetDataSource(assetFileDescriptor.FileDescriptor, assetFileDescriptor.StartOffset, assetFileDescriptor.Length);
                    mediaPlayer.Prepare();

                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    mediaPlayer.Completion += (sender, e) =>
                    {
                        mediaPlayer.Release();
                        taskCompletionSource.SetResult(true); // Signal that playback is complete
                    };

                    mediaPlayer.Start();

                    // Wait for playback to complete asynchronously without blocking
                    await taskCompletionSource.Task;

                    return (true, null); // Success
                }
            }
            catch (FileNotFoundException)
            {
                return (false, $"AudioPlayerForAndroid: The sound file {soundFileName} could not be found in the assets folder.");
            }
            catch (IOException ex)
            {
                return (false, $"AudioPlayerForAndroid: Error playing sound from assets: {ex.Message}");
            }
            catch (Exception ex) // General exception handling
            {
                return (false, $"AudioPlayerForAndroid: a very unexpected error occurred: {ex.Message}, {ex.InnerException}, {ex.GetBaseException}, {ex.Source}, {ex.StackTrace} :::");
            }
        }

    }
}
#endif