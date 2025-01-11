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

    public enum ObstructionType
    {
        /// <summary>
        /// Can walk through, can see through
        /// </summary>
        Open,

        /// <summary>
        /// Cannot walk through, can see through
        /// </summary>
        MovementBlocked,

        /// <summary>
        /// Can walk through, cannot see through
        /// </summary>
        VisionBlocked,

        /// <summary>
        /// Cannot walk through, Cannot see through
        /// </summary>
        FullyBlocked
    }
}
