namespace Roguelike
{
    /// <summary>
    /// Easy access to all const game information
    /// </summary>
    internal static class Constants
    {
        public const string GameTitle = "Roguelike";
        public const string Font = "Fonts/LCD_Tileset.font";
        public const string TileConfiguration = "World/Configuration/tiles.json";
        public const int PlayerFieldOfViewRadius = 6;
        public static (int width, int height) Resolution = (1280, 720);
    }
}
