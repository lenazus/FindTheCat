public static class ImageScaleHelper
{
    public static string GetScaledImagePath(string directory, string imageName)
    {
        var scaleFactors = new[] { "scale-400", "scale-300", "scale-200", "scale-150", "scale-100" };

        foreach (var scale in scaleFactors)
        {
            var scaledFileName = $"{Path.GetFileNameWithoutExtension(imageName)}.{scale}{Path.GetExtension(imageName)}";
            var fullPath = Path.Combine(directory, scaledFileName);

            if (File.Exists(fullPath))
            {
                return fullPath; // Return the first found scaled image
            }
        }

        // Default to original image name if no scaled version is found
        return Path.Combine(directory, imageName);
    }
}