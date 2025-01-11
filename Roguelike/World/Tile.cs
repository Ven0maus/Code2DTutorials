using Roguelike.World.Configuration;
using SadConsole;

namespace Roguelike.World
{
    internal class Tile : ColoredGlyph
    {
        public int X { get; }
        public int Y { get; }
        public ObstructionType Obstruction { get; set; }

        private TileType _tileType;
        public TileType Type
        {
            get => _tileType;
            set
            {
                _tileType = value;

                // Copy over the appearance of the config tile to this tile on tile type change.
                var configurationTile = TilesConfig.Get(Type);
                configurationTile.CopyAppearanceTo(this);

                // We must set this one explicitly because its not part of the appearance, its our custom property.
                Obstruction = configurationTile.Obstruction;
            }
        }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            Type = TileType.None;
        }

        // Used for tile config initialization
        internal Tile(TileType type)
        {
            _tileType = type;
        }
    }
}
