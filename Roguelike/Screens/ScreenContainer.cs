using Roguelike.Entities;
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
        private static ScreenContainer _instance;
        public static ScreenContainer Instance => _instance ?? throw new Exception("ScreenContainer is not yet initialized.");

        public WorldScreen World { get; }
        public ScreenSurface PlayerStats { get; }
        public ScreenSurface Messages { get; }

        public Random Random { get; }

        public ScreenContainer()
        {
            if (_instance != null)
                throw new Exception("Only one ScreenContainer instance can exist.");
            _instance = this;

            Random = new Random();

            // World screen
            World = new WorldScreen(Game.Instance.ScreenCellsX.PercentageOf(70), Game.Instance.ScreenCellsY);
            Children.Add(World);

            // Player stats screen
            PlayerStats = new ScreenSurface(Game.Instance.ScreenCellsX.PercentageOf(30), Game.Instance.ScreenCellsY.PercentageOf(60))
            {
                Position = new Point(World.Position.X + World.Width, World.Position.Y)
            };
            Children.Add(PlayerStats);

            // Messages screen
            Messages = new ScreenSurface(Game.Instance.ScreenCellsX.PercentageOf(30), Game.Instance.ScreenCellsY.PercentageOf(40))
            {
                Position = new Point(World.Position.X + World.Width, PlayerStats.Position.Y + PlayerStats.Height)
            };
            Children.Add(Messages);

            // Temporary for visualization of the surfaces
            PlayerStats.Fill(background: Color.Green);
            Messages.Fill(background: Color.Yellow);
        }
    }
}
