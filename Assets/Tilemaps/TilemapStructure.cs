using Assets.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Tilemaps
{
    public class TilemapStructure : MonoBehaviour
    {
        public int Width, Height, TileSize, Seed;
        private int[] _tiles;
        private Tilemap _graphicMap;

        [Serializable]
        class TileType
        {
            public GroundTileType GroundTile;
            public Color Color;
        }

        [SerializeField]
        private AlgorithmBase[] _algorithms;

        [SerializeField]
        private TileType[] TileTypes;

        private Dictionary<int, Tile> _tileTypeDictionary;

        /// <summary>
        /// Method called by unity automatically.
        /// </summary>
        private void Awake()
        {
            // Retrieve the Tilemap component from the same object this script is attached to
            _graphicMap = GetComponent<Tilemap>();

            // Initialize the one-dimensional array with our map size
            _tiles = new int[Width * Height];

            // Initialize a dictionary lookup table to help us later
            _tileTypeDictionary = new Dictionary<int, Tile>();

            // We need to also assign a texture, so we create one real quick
            var tileSprite = Sprite.Create(new Texture2D(TileSize, TileSize), new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);

            // Create a Tile for each GroundTileType
            foreach (var tiletype in TileTypes)
            {
                // Create a scriptable object instance of type Tile (inherits from TileBase)
                var tile = ScriptableObject.CreateInstance<Tile>();
                // Make sure color is not transparant
                tiletype.Color.a = 1;
                // Set the tile color
                tile.color = tiletype.Color;
                // Assign the sprite we created earlier to our tiles
                tile.sprite = tileSprite;
                // Add to dictionary by key GroundTileType int value, value Tile
                _tileTypeDictionary.Add((int)tiletype.GroundTile, tile);
            }

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
                    tilesArray[x * Width + y] = _tileTypeDictionary[typeOfTile];
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