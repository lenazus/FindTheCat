using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

public static class ApplicationCloser
{
    public static void CloseApplication()
    {
#if ANDROID
        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif WINDOWS
        System.Environment.Exit(0);
#elif IOS
        // iOS doesn't allow terminating the app programmatically,
        // but you can exit gracefully if necessary.
        // throw new InvalidOperationException("Cannot close the app on iOS.");
#endif
    }
}
