using SadConsole;
using SadRogue.Primitives;
using System;

namespace Roguelike.Screens
{
    /// <summary>
    /// Container for all screen objects used by the roguelike game.
    /// </summary>
    internal class ScreenContainer : ScreenObject
    {
        public ScreenSurface World { get; }
        public ScreenSurface PlayerStats { get; }
        public ScreenSurface Messages { get; }

        private static ScreenContainer _instance;
        public static ScreenContainer Instance => _instance ?? throw new Exception("ScreenContainer is not yet initialized.");

        public ScreenContainer()
        {
            if (_instance != null)
                throw new Exception("Only one ScreenContainer instance can exist.");
            _instance = this;

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
