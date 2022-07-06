using Assets.ScriptableObjects;
using System.Collections.Generic;
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

        private HashSet<Vector2Int> _dirtyCoords;
        /// <summary>
        /// True if changes are done in the structure, but have not yet been applied to the tilemap
        /// </summary>
        public bool IsDirty => _dirtyCoords.Count > 0;

        /// <summary>
        /// Method to initialize our tilemap.
        /// </summary>
        public void Initialize()
        {
            // Retrieve the Tilemap component from the same object this script is attached to
            _dirtyCoords = new HashSet<Vector2Int>();
            _graphicMap = GetComponent<Tilemap>();

            // Retrieve the TileGrid component from our parent gameObject
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

            // Update our data so it becomes visible on the map
            UpdateTiles();
        }

        /// <summary>
        /// Updates only a specific position of the structure.
        /// Use this if you only need to update one tile, it is more efficient
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void UpdateTile(int x, int y)
        {
            // Remove coordinate from dirty list
            var coord = new Vector2Int(x, y);
            if (_dirtyCoords.Contains(coord))
            {
                _dirtyCoords.Remove(coord);
            }

            var typeOfTile = GetTile(x, y);
            // Get the ScriptableObject that matches this type and insert it
            Grid.GetTileCache().TryGetValue(typeOfTile, out TileBase tile); // Default return null if not found
            _graphicMap.SetTile(new Vector3Int(x, y, 0), tile);
            _graphicMap.RefreshTile(new Vector3Int(x, y, 0));
        }

        /// <summary>
        /// Updates only a select few positions of the structure.
        /// Use this if you only need to update a few tiles, it is more efficient
        /// </summary>
        public void UpdateTiles(Vector2Int[] positions)
        {
            var positionsArray = new Vector3Int[positions.Length];
            var tilesArray = new TileBase[positions.Length];
            for (int i=0; i < positions.Length; i++)
            {
                var typeOfTile = GetTile(positions[i].x, positions[i].y);
                // Get the ScriptableObject that matches this type and insert it
                Grid.GetTileCache().TryGetValue(typeOfTile, out TileBase tile); // Default return null if not found
                positionsArray[i] = new Vector3Int(positions[i].x, positions[i].y, 0);
                tilesArray[i] = tile;

                // Remove coordinate from dirty list
                if (_dirtyCoords.Contains(positions[i]))
                {
                    _dirtyCoords.Remove(positions[i]);
                }
            }
            _graphicMap.SetTiles(positionsArray, tilesArray);
            foreach (var position in positionsArray)
                _graphicMap.RefreshTile(position);
        }

        /// <summary>
        /// Updates the entire structure, preferably only use this on map initialization
        /// </summary>
        public void UpdateTiles()
        {
            // Create a positions array and tile array required by _graphicMap.SetTiles
            var positionsArray = new Vector3Int[Width * Height];
            var tilesArray = new TileBase[Width * Height];

            // Loop over all our tiles in our data structure
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    positionsArray[x * Width + y] = new Vector3Int(x, y, 0);
                    // Get what tile is at this position
                    var typeOfTile = GetTile(x, y);
                    // Get the ScriptableObject that matches this type and insert it
                    Grid.GetTileCache().TryGetValue(typeOfTile, out TileBase tile); // Default return null if not found
                    tilesArray[x * Width + y] = tile;
                }
            }

            // Clear all dirty coordinates
            _dirtyCoords.Clear();
            _graphicMap.SetTiles(positionsArray, tilesArray);
            _graphicMap.RefreshAllTiles();
        }

        /// <summary>
        /// Returns all 8 neighbors (vertical, horizontal, diagonal)
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <returns></returns>
        public List<KeyValuePair<Vector2Int, int>> GetNeighbors(int tileX, int tileY)
        {
            int startX = tileX - 1;
            int startY = tileY - 1;
            int endX = tileX + 1;
            int endY = tileY + 1;

            var neighbors = new List<KeyValuePair<Vector2Int, int>>();
            for (int x = startX; x < endX + 1; x++)
            {
                for (int y = startY; y < endY + 1; y++)
                {
                    // We don't need to add the tile we are getting the neighbors of.
                    if (x == tileX && y == tileY) continue;

                    // Check if the tile is within the tilemap, otherwise we don't need to pass it along
                    // As it would be an invalid neighbor
                    if (InBounds(x, y))
                    {
                        // Pass along a key value pair of the coordinate + the tile type
                        neighbors.Add(new KeyValuePair<Vector2Int, int>(new Vector2Int(x, y), GetTile(x, y)));
                    }
                }
            }
            return neighbors;
        }

        /// <summary>
        /// Returns only the direct 4 neighbors (horizontal and vertical)
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <returns></returns>
        public List<KeyValuePair<Vector2Int, int>> Get4Neighbors(int tileX, int tileY)
        {
            int startX = tileX - 1;
            int startY = tileY - 1;
            int endX = tileX + 1;
            int endY = tileY + 1;

            var neighbors = new List<KeyValuePair<Vector2Int, int>>();
            for (int x = startX; x < endX + 1; x++)
            {
                if (x == tileX || !InBounds(x, tileY)) continue;
                neighbors.Add(new KeyValuePair<Vector2Int, int>(new Vector2Int(x, tileY), GetTile(x, tileY)));
            }
            for (int y = startY; y < endY + 1; y++)
            {
                if (y == tileY || !InBounds(tileX, y)) continue;
                neighbors.Add(new KeyValuePair<Vector2Int, int>(new Vector2Int(tileX, y), GetTile(tileX, y)));
            }

            return neighbors;
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
        public void SetTile(int x, int y, int? value, bool updateTilemap = false, bool setDirty = true)
        {
            if (InBounds(x, y))
            {
                var prev = _tiles[y * Width + x];
                _tiles[y * Width + x] = value ?? 0;

                // If tile was changed we can update
                if (prev != value)
                {
                    // Add dirty coordinate to list, if modified and it's not yet dirty
                    // Also don't set dirty if we're about to update it too
                    if (!updateTilemap && setDirty)
                    {
                        var coord = new Vector2Int(x, y);
                        if (!_dirtyCoords.Contains(coord))
                            _dirtyCoords.Add(coord);
                    }

                    if (updateTilemap)
                        UpdateTile(x, y);
                }
            }
        }

        /// <summary>
        /// Check if the tile position is valid.
        /// </summary>
        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// Applies the algorithm code to the tilemap structure
        /// </summary>
        /// <param name="algorithm"></param>
        public void Generate(AlgorithmBase algorithm)
        {
            algorithm.Apply(this);
        }
    }
}