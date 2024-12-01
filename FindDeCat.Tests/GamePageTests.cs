using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using FindDeCat.Services;
using FindDeCat.Models;
using FindDeCat.Configuration;
using System.Threading.Tasks;
using System;

namespace FindDeCat.Pages.Tests
{
    [TestClass]
    public class GamePageTests
    {
        private Mock<IPickerHandler> _pickerHandlerMock;
        private Mock<IGameUIService> _gameUIServiceMock;
        private Mock<IAutoClickService> _autoClickServiceMock;
        private GameState _gameState;
        private GamePage _gamePage;

        // Mock Picker class to avoid MAUI initialization issues
        private class MockPicker : Picker
        {
            public MockPicker() : base() { }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // Create mock dependencies
            _pickerHandlerMock = new Mock<IPickerHandler>();
            _gameUIServiceMock = new Mock<IGameUIService>();
            _autoClickServiceMock = new Mock<IAutoClickService>();
            _gameState = new GameState();

            // Create a mock GamePage that allows us to override component initialization
            var mockGamePage = new Mock<GamePage>(
                _pickerHandlerMock.Object,
                _gameState,
                _gameUIServiceMock.Object,
                _autoClickServiceMock.Object
            )
            { CallBase = true };

            // Setup mock pickers
            var languagePicker = new MockPicker();
            var categoryPicker = new MockPicker();
            var delayPicker = new MockPicker();

            // Setup method to return mock pickers
            mockGamePage.Setup(x => x.GetLanguagePicker()).Returns(languagePicker);
            mockGamePage.Setup(x => x.GetCategoryPicker()).Returns(categoryPicker);
            mockGamePage.Setup(x => x.GetDelayPicker()).Returns(delayPicker);

            // Create the game page
            _gamePage = mockGamePage.Object;
        }

        [TestMethod]
        public void InitializePickers_ShouldCallPickerHandlerMethods()
        {
            // Arrange
            var languagePicker = _gamePage.GetLanguagePicker();
            var categoryPicker = _gamePage.GetCategoryPicker();
            var delayPicker = _gamePage.GetDelayPicker();

            // Act
            _gamePage.InitializePickers();

            // Assert
            _pickerHandlerMock.Verify(
                x => x.InitializeLanguagePicker(It.Is<Picker>(p => p == languagePicker)),
                Times.Once
            );
            _pickerHandlerMock.Verify(
                x => x.InitializeCategoryPicker(It.Is<Picker>(p => p == categoryPicker)),
                Times.Once
            );
            _pickerHandlerMock.Verify(
                x => x.InitializeDelayPicker(It.Is<Picker>(p => p == delayPicker)),
                Times.Once
            );
        }

    }
}