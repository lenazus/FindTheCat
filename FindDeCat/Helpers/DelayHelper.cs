public static class DelayHelper
{
    public static async Task DelayAfterDisplayImage(int delayOption, int defaultDelay)
    {
        var delay = delayOption > 0 ? delayOption * 1000 : defaultDelay;

        if (delay > 0)
        {
            await Task.Delay(delay);
        }
    }
}
