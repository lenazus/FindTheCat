using FindDeCat.Services;

public interface IAutoClickService
{
    bool IsAutoClickEnabled { get; }
    void ToggleAutoClick(IButtonTextUpdater autoClickButton, Image[] rectangles, Action<Image> performClickAction);
    void PauseAutoClick();
}