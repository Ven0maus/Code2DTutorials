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

        [SerializeField]
        private TileTypes.GroundTiles[] GroundTileTypes;
        [SerializeField]
        private TileTypes.ObjectTiles[] ObjectTileTypes;

        private Dictionary<int, Tile> _tiles;
        private Dictionary<TilemapType, TilemapStructure> _tilemaps;

        private void Awake()
        {
            InitializeTiles();

            _tilemaps = new Dictionary<TilemapType, TilemapStructure>();

            // Add all our tilemaps by name to collection, so we can access them easily.
            foreach (Transform child in transform)
            {
                var tilemap = child.GetComponent<TilemapStructure>();
                if (tilemap == null) continue;
                if (_tilemaps.ContainsKey(tilemap.Type))
                {
                    throw new Exception("A duplicate tilemap of type: " + tilemap.Type + " exists in the scene.");
                }
                _tilemaps.Add(tilemap.Type, tilemap);
            }

            // Let's initialize our tilemaps now that they are in the collection.
            foreach (var tilemap in _tilemaps.Values)
            {
                tilemap.Initialize();
            }
        }

        /// <summary>
        /// Returns all the cached shared tiles available to be placed on the tilemap
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Tile> GetTileCache()
        {
            return _tiles;
        }

        /// <summary>
        /// Returns the tilemap of the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TilemapStructure GetTilemap(TilemapType type)
        {
            if (!_tilemaps.TryGetValue(type, out TilemapStructure structure))
                throw new Exception($"This grid does not contain a tilemap of type {type}.");
            return structure;
        }

        private void InitializeTiles()
        {
            _tiles = new Dictionary<int, Tile>
            {
                // Add default void tile
                { 0, null }
            };

            // Add all tilesets here
            AddTileSet(_tiles, GroundTileTypes);
            AddTileSet(_tiles, ObjectTileTypes);
        }

        /// <summary>
        /// Use this method to add a new tileset to the dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="tileData"></param>
        /// <exception cref="Exception"></exception>
        private void AddTileSet(Dictionary<int, Tile> tiles, TileTypes.TileData[] tileData)
        {
            foreach (var tiletype in tileData)
            {
                if (tiletype.TileTypeId == 0) continue;

                // If we have a custom tile, use it otherwise create a new tile
                var tile = tiletype.Tile == null ?
                    CreateTile(tiletype.Color, tiletype.Sprite) :
                    tiletype.Tile;
                tile.colliderType = tiletype.ColliderType;
                
                // Check if the tile id already exists in the tiles
                if (tiles.ContainsKey(tiletype.TileTypeId))
                {
                    var tileTypeInfo = GetTileTypeInfo(tiletype);
                    throw new Exception($"Error adding tile from enum [{tileTypeInfo.Item1}]: Tile Id '{tiletype.TileTypeId}:{tileTypeInfo.Item2}' already exists in another tile enum.");
                }

                tiles.Add(tiletype.TileTypeId, tile);
            }
        }

        /// <summary>
        /// Uses reflection to retrieve the type information from the TileData
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private (string, string) GetTileTypeInfo(TileTypes.TileData data)
        {
            var type = data.GetType();
            var tileTypeField = type.GetField("TileType", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var value = tileTypeField.GetValue(data);
            return (tileTypeField.FieldType.Name, value.ToString());
        }

        private Tile CreateTile(Color color, Sprite sprite)
        {
            // Create an instance of type Tile (inherits from TileBase)
            var tile = ScriptableObject.CreateInstance<Tile>();

            // No sprite specified, we create one for the color instead
            if (sprite == null)
            {
                // Created sprites do not support custom physics shape
                var texture = new Texture2D(TileSize, TileSize)
                {
                    filterMode = FilterMode.Point
                };

                // Create new sprite without any custom physics shape
                sprite = Sprite.Create(texture, new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);

                // Make sure color is not transparant and set color to the tile
                color.a = 1;
                tile.color = color;
            }

            tile.sprite = sprite;

            return tile;
        }
    }
}
