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
            Settings.ResizeMode = Settings.WindowResizeOptions.Stretch;

            Builder configuration = new();

            _ = configuration
                .SetScreenSize(60, 40)
                .SetStartingScreen<ScreenContainer>()
                .IsStartingScreenFocused(true)
                .ConfigureFonts((f, gh) =>
                {
                    f.UseCustomFont(Constants.Font);

                    // A bit of a hacky solution, but we read the font json file to determine the width/height of the font glyphs
                    // This way we can determine how many cells can fit in our resolution and we pass that as our screen size in cells.
                    // We do it in ConfigureFonts because here the GameHost.Instance property is not null, which is needed for the serializer.
                    var (width, height) = DefineScreenSizeByResolution();
                    configuration.SetScreenSize(width, height);
                })
                .OnStart(GameStart);

            Game.Create(configuration);
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        private static void GameStart(object sender, GameHost e)
        {
            ScreenContainer.Instance.World.Generate();
        }

        private static (int width, int height) DefineScreenSizeByResolution()
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            using var font = Serializer.Load<IFont>(Constants.Font, false, settings);
            return (Constants.Resolution.width / font.GlyphWidth, Constants.Resolution.height / font.GlyphHeight);
        }
    }
}
