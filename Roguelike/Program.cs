using Roguelike.Screens;
using SadConsole;
using SadConsole.Configuration;

namespace Roguelike
{
    /// <summary>
    /// Process startup
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            Settings.WindowTitle = Constants.GameTitle;
            Settings.ResizeMode = Settings.WindowResizeOptions.Fit;

            Builder configuration = new Builder()
                .SetScreenSize(90, 30)
                .SetStartingScreen<ScreenContainer>()
                .IsStartingScreenFocused(true)
                .ConfigureFonts(true);

            Game.Create(configuration);
            Game.Instance.Run();
            Game.Instance.Dispose();
        }
    }
}
