using System.Collections.Generic;
using FindDeCat.Models;
using FindDeCat.Pages;

namespace FindDeCat.Services
{
    public interface IGameUIService
    {
        Task HandleRectangleTappedAsync(Image tappedRectangle, Label gameLabel, Image[] rectangles, Models.GameState gameState);
        Task InitializeGame(GameState gameState, Image[] rectangles, Label gameLabel);
        Task RestoreOriginalUI(Image[] rectangles, Label gameLabel);
    }
}