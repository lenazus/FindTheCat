using System;
using System.Linq;
using System.Threading;
using Xunit;
using Moq;
using FindDeCat.Services;
using FindDeCat.Configuration;
using Microsoft.Maui.Controls; //depending on your project

[TestClass]
public class AutoClickServiceTests
{
    [TestMethod]
    public void PauseAutoClick_NoEffect_WhenAlreadyDisabled()
    {
        // Arrange
        var autoClickService = new AutoClickService();

        // Act
        autoClickService.PauseAutoClick();

        // Assert
        Assert.IsFalse(autoClickService.IsAutoClickEnabled); // Ensure it stays disabled
    }

    [TestMethod]
    public void ToggleAutoClick_EnablesAutoClick()
    {
        // Arrange
        var autoClickService = new AutoClickService();
        var buttonTextUpdater = new Mock<IButtonTextUpdater>();
        buttonTextUpdater.SetupProperty(b => b.Text, "PLAY");
        var rectangles = new[] { new Mock<Image>().Object, new Mock<Image>().Object };
        var performClickAction = new Mock<Action<Image>>();

        // Act
        autoClickService.ToggleAutoClick(buttonTextUpdater.Object, rectangles, performClickAction.Object);

        // Assert
        Assert.IsTrue(autoClickService.IsAutoClickEnabled);
        Assert.AreEqual(AppConfiguration.GameSettings.PAUSE, buttonTextUpdater.Object.Text);
    }

    [TestMethod]
    public void ToggleAutoClick_DisablesAutoClick()
    {
        // Arrange
        var autoClickService = new AutoClickService();
        var buttonTextUpdater = new Mock<IButtonTextUpdater>();
        buttonTextUpdater.SetupProperty(b => b.Text, "PLAY");
        var rectangles = new[] { new Mock<Image>().Object, new Mock<Image>().Object };
        var performClickAction = new Mock<Action<Image>>();

        // Enable auto-click first
        autoClickService.ToggleAutoClick(buttonTextUpdater.Object, rectangles, performClickAction.Object);

        // Act
        autoClickService.ToggleAutoClick(buttonTextUpdater.Object, rectangles, performClickAction.Object);

        // Assert
        Assert.False(autoClickService.IsAutoClickEnabled);
        Assert.Equal(AppConfiguration.GameSettings.PLAY, buttonTextUpdater.Object.Text);
    }

    [TestMethod]
    public void PauseAutoClick_StopsAutoClick_WhenEnabled()
    {
        // Arrange
        var autoClickService = new AutoClickService();
        var buttonTextUpdater = new Mock<IButtonTextUpdater>();
        buttonTextUpdater.SetupProperty(b => b.Text, "PAUSE");
        var rectangles = new[] { new Mock<Image>().Object, new Mock<Image>().Object };
        var performClickAction = new Mock<Action<Image>>();

        // Enable auto-click first
        autoClickService.ToggleAutoClick(buttonTextUpdater.Object, rectangles, performClickAction.Object);

        // Act
        autoClickService.PauseAutoClick();

        // Assert
        Assert.False(autoClickService.IsAutoClickEnabled);
        buttonTextUpdater.VerifySet(b => b.Text = AppConfiguration.GameSettings.PLAY, Times.Once);
    }

    [TestMethod]
    public void PerformRandomRectangleClick_CallsPerformClickAction()
    {
        // Arrange
        var autoClickService = new AutoClickService();
        var rectangles = new[] { new Mock<Image>().Object, new Mock<Image>().Object, new Mock<Image>().Object };
        var performClickAction = new Mock<Action<Image>>();

        // Act
        var methodInfo = typeof(AutoClickService)
            .GetMethod("PerformRandomRectangleClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        methodInfo.Invoke(autoClickService, new object[] { rectangles, performClickAction.Object });

        // Assert
        performClickAction.Verify(action => action(It.IsAny<Image>()), Times.Once);
    }

    [TestMethod]
    public void StartAutoClick_StartsTimer()
    {
        // Arrange
        var autoClickService = new AutoClickService();
        var buttonTextUpdater = new Mock<IButtonTextUpdater>();
        buttonTextUpdater.SetupProperty(b => b.Text, "PLAY");
        var rectangles = new[] { new Mock<Image>().Object, new Mock<Image>().Object };
        var performClickAction = new Mock<Action<Image>>();

        // Enable auto-click
        autoClickService.ToggleAutoClick(buttonTextUpdater.Object, rectangles, performClickAction.Object);

        // Act
        // Allow the timer to trigger at least once
        Thread.Sleep(2000); // Note: Using Thread.Sleep is not ideal for unit tests

        // Assert
        performClickAction.Verify(action => action(It.IsAny<Image>()), Times.AtLeastOnce);
    }
}
