using Roguelike.Entities.Actors;
using SadConsole;
using SadRogue.Primitives;

namespace Roguelike.Screens
{
    internal class PlayerStatsScreen : ScreenSurface
    {
        private static Player Player => ScreenContainer.Instance.World.Player;

        public PlayerStatsScreen(int width, int height) : base(width, height)
        { }

        public void UpdatePlayerStats()
        {
            Surface.Clear();
            Surface.DrawBorderWithTitle("Attributes", Color.Gray, Color.Magenta);
            DrawPlayerAttributes();
        }

        private void DrawPlayerAttributes()
        {
            Surface.Print(2, 2, $"HP:    {Player.Stats.Health}/{Player.Stats.MaxHealth}");
            Surface.Print(2, 3, $"ATK:   {Player.Stats.Attack}");
            Surface.Print(2, 4, $"DEF:   {Player.Stats.Defense}");
            Surface.Print(2, 5, $"AGI:   {Player.Stats.DodgeChance}");
            Surface.Print(2, 6, $"CRIT:  {Player.Stats.CritChance}");
        }
    }
}
