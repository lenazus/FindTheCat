using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;

namespace FindDeCat.Services.Tests
{
    [TestClass]
    public class TranslationsTests
    {
        private Mock<ITranslations> _translationsMock;
        private ITranslations _translations;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock the Translations class for testing
            _translationsMock = new Mock<ITranslations>();
            _translations = _translationsMock.Object;
        }

        #region GetEmoji Tests

        [TestMethod]
        public void GetEmoji_ShouldReturnEmoji_WhenKeyExists()
        {
            // Arrange
            _translationsMock.Setup(t => t.GetEmoji(It.IsAny<string>())).Returns("🍎");

            // Act
            var emoji = _translations.GetEmoji("apple");

            // Assert
            emoji.Should().Be("🍎");
        }

        [TestMethod]
        public void GetEmoji_ShouldReturnEmpty_WhenKeyDoesNotExist()
        {
            // Arrange
            _translationsMock.Setup(t => t.GetEmoji(It.IsAny<string>())).Returns(string.Empty);

            // Act
            var emoji = _translations.GetEmoji("unknown");

            // Assert
            emoji.Should().BeEmpty();
        }

        #endregion
    }
}