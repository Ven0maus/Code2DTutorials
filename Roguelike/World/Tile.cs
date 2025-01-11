using SadConsole;

namespace Roguelike.World
{
    internal class Tile : ColoredGlyph
    {
        public int X { get; }
        public int Y { get; }
        public ObstructionType Obstruction { get; set; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
