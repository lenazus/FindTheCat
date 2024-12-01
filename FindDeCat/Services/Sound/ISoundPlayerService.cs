namespace FindDeCat.Services.Sound
{
    public interface ISoundPlayerService
    {
        Task<(bool Success, string ErrorMessage)> PlaySoundAsync(string soundFileName);
    }
}