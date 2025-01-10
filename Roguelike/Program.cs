using Newtonsoft.Json;
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

            Builder configuration = new();

            configuration = configuration
                .SetStartingScreen<ScreenContainer>()
                .IsStartingScreenFocused(true)
                .ConfigureFonts((f, gh) =>
                {
                    f.UseCustomFont(Constants.Font);

                    // Set screen size based on the font
                    var (width, height) = DefineScreenSize();
                    configuration.SetScreenSize(width, height);
                });

            Game.Create(configuration);
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        private static (int width, int height) DefineScreenSize()
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var font = Serializer.Load<IFont>(Constants.Font, false, settings);
            return (Constants.Resolution.width / font.GlyphWidth, Constants.Resolution.height / font.GlyphHeight);
        }
    }
}
