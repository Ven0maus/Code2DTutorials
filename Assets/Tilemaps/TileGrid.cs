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
        class GroundTiles : ITileData
        {
            public GroundTileType TileType;
        }

        [Serializable]
        class ObjectTiles : ITileData
        {
            public ObjectTileType TileType;
        }

        class ITileData
        {
            public Sprite Sprite;
            public Color Color;
            public Tile Tile;
            public Tile.ColliderType ColliderType;
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

            foreach (var tiletype in GroundTileTypes)
            {
                if (tiletype.TileType == 0) continue;

                // If we have a custom tile, use it otherwise create a new tile
                var tile = tiletype.Tile == null ?
                    CreateTile(tiletype.Color, tiletype.Sprite, tiletype.ColliderType) :
                    tiletype.Tile;

                dictionary.Add((int)tiletype.TileType, tile);
            }

            foreach (var tiletype in ObjectTileTypes)
            {
                if (tiletype.TileType == 0) continue;

                // If we have a custom tile, use it otherwise create a new tile
                var tile = tiletype.Tile == null ?
                    CreateTile(tiletype.Color, tiletype.Sprite, tiletype.ColliderType) :
                    tiletype.Tile;

                dictionary.Add((int)tiletype.TileType, tile);
            }

            return dictionary;
        }

        private Tile CreateTile(Color color, Sprite sprite, Tile.ColliderType colliderType)
        {
            // No sprite specified, we create one for the color instead
            bool setColor = false;
            Texture2D texture = sprite == null ? null : sprite.texture;
            if (texture == null)
            {
                setColor = true;
                // Created sprites do not support custom physics shape
                texture = new Texture2D(TileSize, TileSize)
                {
                    filterMode = FilterMode.Point
                };
                sprite = Sprite.Create(texture, new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);
            }

            // Create an instance of type Tile (inherits from TileBase)
            var tile = ScriptableObject.CreateInstance<Tile>();

            if (setColor)
            {
                // Make sure color is not transparant
                color.a = 1;
                // Set the tile color
                tile.color = color;
            }

            // Make sure the collider type is Sprite to use
            // Custom physics shape for collider shape
            tile.colliderType = colliderType;
            tile.sprite = sprite;

            return tile;
        }
    }
}
