using Roguelike.World.Configuration;
using SadConsole;
using SadRogue.Primitives;

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
                if (_tileType != value)
                {
                    _tileType = value;
                    CopyFromConfiguration();
                    IsDirty = true;
                }
            }
        }

        private bool _inFov;
        public bool InFov
        {
            get => _inFov;
            set
            {
                if (_inFov != value)
                {
                    _inFov = value;

                    if (_inFov)
                    {
                        Foreground = _seenForeground;
                        Background = _seenBackground;
                    }
                    else
                    {
                        Foreground = _unseenForeground;
                        Background = _unseenBackground;
                    }
                }
            }
        }

        private Color _seenForeground, _seenBackground, _unseenForeground, _unseenBackground;

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            IsVisible = false;
            CopyFromConfiguration();
        }

        // Used for tile config initialization
        internal Tile(TileType type)
        {
            _tileType = type;
        }

        private void CopyFromConfiguration()
        {
            // Copy over the appearance of the config tile to this tile on tile type change.
            var configurationTile = TilesConfig.Get(Type);
            configurationTile.CopyAppearanceTo(this);

            // We must set this one explicitly because its not part of the appearance, its our custom property.
            Obstruction = configurationTile.Obstruction;

            // Set colors for FOV
            SetColorsForFOV();
        }

        private void SetColorsForFOV()
        {
            _seenForeground = Foreground;
            _seenBackground = Background;
            _unseenForeground = Color.Lerp(_seenForeground, Color.Black, 0.5f);
            _unseenBackground = Color.Lerp(_seenBackground, Color.Black, 0.5f);
        }
    }
}
