using SadConsole;
using SadRogue.Primitives;

namespace Roguelike.Screens
{
    /// <summary>
    /// Container for all screen objects used by the roguelike game.
    /// </summary>
    internal class ScreenContainer : ScreenObject
    {
        public ScreenSurface World { get; set; }
        public ScreenSurface PlayerStats { get; set; }
        public ScreenSurface Messages { get; set; }

        public ScreenContainer()
        {
            // World screen
            World = new ScreenSurface(Game.Instance.ScreenCellsX.Percent(70), Game.Instance.ScreenCellsY);
            Children.Add(World);

            // Player stats screen
            PlayerStats = new ScreenSurface(Game.Instance.ScreenCellsX.Percent(30), Game.Instance.ScreenCellsY.Percent(60))
            {
                Position = new Point(World.Position.X + World.Width, World.Position.Y)
            };
            Children.Add(PlayerStats);

            // Messages screen
            Messages = new ScreenSurface(Game.Instance.ScreenCellsX.Percent(30), Game.Instance.ScreenCellsY.Percent(40))
            {
                Position = new Point(World.Position.X + World.Width, PlayerStats.Position.Y + PlayerStats.Height)
            };
            Children.Add(Messages);

            // Temporary for visualization of the surfaces
            World.Fill(background: Color.Blue);
            PlayerStats.Fill(background: Color.Green);
            Messages.Fill(background: Color.Yellow);
        }
    }
}
