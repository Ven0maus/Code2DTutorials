using Assets.ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Tilemaps
{
    public enum TilemapType
    {
        Ground,
        Object
    }

    public class TilemapStructure : MonoBehaviour
    {
        [SerializeField]
        private TilemapType _type;
        public TilemapType Type { get { return _type; } }

        [HideInInspector]
        public int Width, Height;

        private int[] _tiles;
        private Tilemap _graphicMap;

        [HideInInspector]
        public TileGrid Grid;

        [SerializeField]
        private AlgorithmBase[] _algorithms;

        /// <summary>
        /// Method to initialize our tilemap.
        /// </summary>
        public void Initialize()
        {
            // Retrieve the Tilemap component from the same object this script is attached to
            _graphicMap = GetComponent<Tilemap>();

            // Retrive the TileGrid component from our parent gameObject
            Grid = transform.parent.GetComponent<TileGrid>();

            // Get width and height from parent
            Width = Grid.Width;
            Height = Grid.Height;

            // Initialize the one-dimensional array with our map size
            _tiles = new int[Width * Height];

            // Apply all the algorithms to the tilemap
            foreach (var algorithm in _algorithms)
            {
                Generate(algorithm);
            }

            // Render our data
            RenderAllTiles();
        }

        /// <summary>
        /// Renders the entire data structure to unity's tilemap.
        /// </summary>
        public void RenderAllTiles()
        {
            // Create a positions array and tile array required by _graphicMap.SetTiles
            var positionsArray = new Vector3Int[Width * Height];
            var tilesArray = new Tile[Width * Height];

            // Loop over all our tiles in our data structure
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    positionsArray[x * Width + y] = new Vector3Int(x, y, 0);
                    // Get what tile is at this position
                    var typeOfTile = GetTile(x, y);
                    // Get the ScriptableObject that matches this type and insert it
                    if (!Grid.Tiles.TryGetValue(typeOfTile, out Tile tile))
                    {
                        if (typeOfTile != 0)
                        {
                            Debug.LogError("Tile not defined for id: " + typeOfTile);
                        }

                        tilesArray[x * Width + y] = null;
                        continue;
                    }
                    tilesArray[x * Width + y] = tile;
                }
            }

            _graphicMap.SetTiles(positionsArray, tilesArray);
            _graphicMap.RefreshAllTiles();
        }

        /// <summary>
        /// Return type of tile, otherwise 0 if invalid position.
        /// </summary>
        public int GetTile(int x, int y)
        {
            return InBounds(x, y) ? _tiles[y * Width + x] : 0;
        }

        /// <summary>
        /// Set type of tile at the given position.
        /// </summary>
        public void SetTile(int x, int y, int value)
        {
            if (InBounds(x, y))
            {
                _tiles[y * Width + x] = value;
            }
        }

        /// <summary>
        /// Check if the tile position is valid.
        /// </summary>
        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void Generate(AlgorithmBase algorithm)
        {
            algorithm.Apply(this);
        }
    }
}