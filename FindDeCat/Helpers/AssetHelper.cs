using System;
using System.IO;

namespace FindDeCat.Helpers
{
    public static class AssetHelper
    {
        public static bool AssetFileExists(string fileName)
        {
#if ANDROID
            using (var assets = Android.App.Application.Context.Assets)
            {
                try
                {
                    using (var stream = assets.Open(fileName))
                    {
                        return true;
                    }
                }
                catch (Java.IO.IOException)
                {
                    return false;
                }
                return false;
            }
#else
            return File.Exists(Path.Combine(AppContext.BaseDirectory, fileName));
#endif
        }
    }
}
