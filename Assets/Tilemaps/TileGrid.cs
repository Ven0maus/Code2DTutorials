using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Tilemaps
{
    public class TileGrid : MonoBehaviour
    {
        public int Width, Height;
        public int TileSize, Seed;
        public Dictionary<int, Tile> Tiles { get; private set; }

        [Serializable]
        class GroundTiles
        {
            public GroundTileType TileType;
            public Texture2D Texture;
            public Color Color;
        }

        [Serializable]
        class ObjectTiles
        {
            public ObjectTileType TileType;
            public Texture2D Texture;
            public Color Color;
        }

        [SerializeField]
        private GroundTiles[] GroundTileTypes;
        [SerializeField]
        private ObjectTiles[] ObjectTileTypes;

        public Dictionary<TilemapType, TilemapStructure> Tilemaps;

        private void Awake()
        {
            Tiles = InitializeTiles();

            Tilemaps = new Dictionary<TilemapType, TilemapStructure>();

            // Add all our tilemaps by name to collection, so we can access them easily.
            foreach (Transform child in transform)
            {
                var tilemap = child.GetComponent<TilemapStructure>();
                if (tilemap == null) continue;
                if (Tilemaps.ContainsKey(tilemap.Type))
                {
                    throw new Exception("Duplicate tilemap type: " + tilemap.Type);
                }
                Tilemaps.Add(tilemap.Type, tilemap);
            }

            // Let's initialize our tilemaps now that they are in the collection.
            foreach (var tilemap in Tilemaps.Values)
            {
                tilemap.Initialize();
            }
        }

        private Dictionary<int, Tile> InitializeTiles() 
        {
            var dictionary = new Dictionary<int, Tile>();

            // Create a Tile for each GroundTileType
            foreach (var tiletype in GroundTileTypes)
            {
                // Don't make a tile for empty
                if (tiletype.TileType == 0) continue;
                // Create tile scriptable object
                var tile = CreateTile(tiletype.Color, tiletype.Texture);
                // Add to dictionary by key int value, value Tile
                dictionary.Add((int)tiletype.TileType, tile);
            }

            // Create a Tile for each ObjectTileType
            foreach (var tiletype in ObjectTileTypes)
            {
                // Don't make a tile for empty
                if (tiletype.TileType == 0) continue;
                // Create tile scriptable object
                var tile = CreateTile(tiletype.Color, tiletype.Texture);
                // Add to dictionary by key int value, value Tile
                dictionary.Add((int)tiletype.TileType, tile);
            }

            return dictionary;
        }

        private Tile CreateTile(Color color, Texture2D texture)
        {
            // If we haven't specified one, we just create an empty one for the color instead
            bool setColor = false;
            if (texture == null)
            {
                setColor = true;
                texture = new Texture2D(TileSize, TileSize);
            }

            // We should be using Point mode, to get the most quality out of our tiles
            texture.filterMode = FilterMode.Point;

            // Create our sprite with the texture passed along
            var sprite = Sprite.Create(texture, new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);

            // Create a scriptable object instance of type Tile (inherits from TileBase)
            var tile = ScriptableObject.CreateInstance<Tile>();

            if (setColor)
            {
                // Make sure color is not transparant
                color.a = 1;
                // Set the tile color
                tile.color = color;
            }

            // Assign the sprite we created earlier to our tiles
            tile.sprite = sprite;

            return tile;
        }
    }
}
