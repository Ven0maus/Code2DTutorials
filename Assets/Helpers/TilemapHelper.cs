using Assets.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Helpers
{
    public static class TilemapHelper
    {
        public static List<Vector2Int> GetTilesByType(TilemapStructure tilemap, IEnumerable<int> enumerable)
        {
            // Best to ToList() the IEnumerable because they will otherwise cause multiple enumerations.
            var tileTypes = enumerable.ToList();
            var validTilePositions = new List<Vector2Int>();
            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    var tileType = tilemap.GetTile(x, y);
                    // Here we use Any to check if any of the tile types match the current tile
                    if (tileTypes.Any(a => a == tileType))
                    {
                        validTilePositions.Add(new Vector2Int(x, y));
                    }
                }
            }
            return validTilePositions;
        }

        public static Vector2Int? FindClosestTileByType(TilemapStructure tilemap, Vector2Int startPos, int tileType)
        {
            float smallestDistance = float.MaxValue;
            Vector2Int? smallestDistancePosition = null;
            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    if (tilemap.GetTile(x, y) == tileType)
                    {
                        // Here we check the distance between the start position and the current tile
                        float distance = ((startPos.x - x) * (startPos.x - x) + (startPos.y - y) * (startPos.y - y));
                        // If this distance is smaller, than the smallest one we have so far encountered
                        // Then let's update it
                        if (distance < smallestDistance)
                        {
                            smallestDistance = distance;
                            smallestDistancePosition = new Vector2Int(x, y);
                        }
                    }
                }
            }
            return smallestDistancePosition;
        }
    }
}
