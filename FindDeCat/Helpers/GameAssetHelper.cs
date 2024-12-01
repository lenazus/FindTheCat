public static class GameAssetHelper
{
    public static string GetSoundFileName(string name, string languageCode)
    {
        return $"{name.Replace(" ", "-")}_{languageCode}.mp3";
    }

    public static string GetImageFileName(string name)
    {
        return $"{name}.png".ToLower();
    }

    public static string GetDisplayName(string name)
    {
        return name.ToUpper();
    }
}