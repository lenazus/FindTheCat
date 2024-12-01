namespace FindDeCat.Services.Sound
{
    public interface IAudioPlayer
    {
        Task<(bool Success, string ErrorMessage)> PlaySoundAsync(string soundFilePath);
    }
}