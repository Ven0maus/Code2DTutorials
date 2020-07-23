using Assets.Helpers;
using Assets.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DownstreamRiverGeneration", menuName = "Algorithms/DownstreamRiverGeneration")]
    public class DownstreamRiverGeneration : AlgorithmBase
    {
        public int MinRiverQuota;
        public int MaxRiverQuota;
        public int MinDistanceBetweenRiverStartPoints;
        public GroundTileType[] StartingTileTypes;
        public NoiseGeneration GroundHeightmap;

        private System.Random _random;

        public override void Apply(TilemapStructure tilemap)
        {
            _random = new System.Random(tilemap.Grid.Seed);

            // Re-create the heightmap from our tilemap
            var heightmap = Noise.GenerateNoiseMap(tilemap.Width, tilemap.Height, tilemap.Grid.Seed, GroundHeightmap.NoiseScale, GroundHeightmap.Octaves, GroundHeightmap.Persistance, GroundHeightmap.Lacunarity, GroundHeightmap.Offset);
            if (GroundHeightmap.ApplyIslandGradient)
            {
                var islandGradient = Noise.GenerateIslandGradientMap(tilemap.Width, tilemap.Height);
                for (int x = 0, y; x < tilemap.Width; x++)
                {
                    for (y = 0; y < tilemap.Height; y++)
                    {
                        // Subtract the islandGradient value from the noiseMap value
                        float subtractedValue = heightmap[y * tilemap.Width + x] - islandGradient[y * tilemap.Width + x];

                        // Apply it into the map, but make sure we clamp it between 0f and 1f
                        heightmap[y * tilemap.Width + x] = Mathf.Clamp01(subtractedValue);
                    }
                }
            }

            // Get all start positions
            var validStartPositions = TilemapHelper.GetTilesByType(tilemap, StartingTileTypes.Select(a => (int)a));
            var amountOfRivers = _random.Next(MinRiverQuota, MaxRiverQuota + 1);

            var rivers = new List<DownstreamRiver>();
            for (int i = 0; i < amountOfRivers; i++)
            {
                // Get valid startPoint with respect to distance between existing rivers
                var startPoint = GetValidStartPosition(validStartPositions, rivers);
                if (!startPoint.HasValue) break;

                // Build river from start based on heightmap
                var river = new DownstreamRiver(startPoint.Value);
                if (river.Build(tilemap, heightmap))
                {
                    rivers.Add(river);
                }
            }

            // Set river tiles into tilemap
            int riverTileId = (int)GroundTileType.River;
            foreach (var riverPosition in rivers.SelectMany(a => a.RiverPositions))
            {
                tilemap.SetTile(riverPosition.x, riverPosition.y, riverTileId);
            }
        }

        private Vector2Int? GetValidStartPosition(List<Vector2Int> startPositions, List<DownstreamRiver> rivers)
        {
            // If no tiles available return null
            if (!startPositions.Any()) return null;

            // Get a random starting tile
            var startPoint = startPositions[_random.Next(0, startPositions.Count)];
            startPositions.Remove(startPoint);

            // Also here we have an attempt check
            const int maxAttempts = 500;
            int attempt = 0;

            // While there is any river where it's startpoint isn't enough distance away from our new startPoint
            while (rivers.Any(river => !river.CheckDistance(startPoint, MinDistanceBetweenRiverStartPoints)))
            {
                if (attempt >= maxAttempts)
                {
                    return null;
                }
                attempt++;

                // Get the next random tile
                if (!startPositions.Any()) return null;
                startPoint = startPositions[_random.Next(0, startPositions.Count)];
                startPositions.Remove(startPoint);
            }

            return startPoint;
        }

        class DownstreamRiver
        {
            public Vector2Int StartPos;
            public HashSet<Vector2Int> RiverPositions;

            private const int _maxAttempts = 1000;

            public DownstreamRiver(Vector2Int startPos)
            {
                StartPos = startPos;
                RiverPositions = new HashSet<Vector2Int> { StartPos };
            }

            public bool CheckDistance(Vector2Int startPos, int minDistance)
            {
                float distance = ((startPos.x - StartPos.x) * (startPos.x - StartPos.x) + (startPos.y - StartPos.y) * (startPos.y - StartPos.y));
                return distance > minDistance * minDistance;
            }

            public bool Build(TilemapStructure tilemap, float[] heightmap)
            {
                Vector2Int currentPos = RiverPositions.First();

                // The target tile we want the river to attempt to reach
                int waterTileId = (int)GroundTileType.DeepWater;

                bool done = false;
                int attempt = 0;
                while (!done)
                {
                    // Check how many attempts we have done so far
                    if (attempt >= _maxAttempts)
                    {
                        break;
                    }
                    attempt++;

                    // Get the height of the current position
                    var height = heightmap[currentPos.y * tilemap.Width + currentPos.x];

                    // Find the neighbor with the lowest height that isn't already a part of the river
                    // Here we use a dirty trick to create a nullable struct, so FirstOrDefault can properly return null
                    // Incase we cannot get to a water tile
                    var lowestHeightNeighbor = tilemap.Get4Neighbors(currentPos.x, currentPos.y)
                        .Select(a => new KeyValuePair<Vector2Int, float>(a.Key, heightmap[a.Key.y * tilemap.Width + a.Key.x]))
                        .OrderBy(a => a.Value)
                        .Select(a => new KeyValuePair<Vector2Int, float>?(a))
                        .FirstOrDefault(a => !RiverPositions.Contains(a.Value.Key));

                    // If the lowest neighbor is null
                    if (lowestHeightNeighbor == null)
                    {
                        // Can't go deeper downwards, we made a lake.
                        done = true;
                        break;
                    }

                    // Add the current pos to the river positions
                    currentPos = lowestHeightNeighbor.Value.Key;
                    RiverPositions.Add(lowestHeightNeighbor.Value.Key);

                    // Check if we are done, by checking if the current pos tile is a water tile
                    done = tilemap.GetTile(lowestHeightNeighbor.Value.Key.x, lowestHeightNeighbor.Value.Key.y) == waterTileId;
                }

                return done;
            }
        }
    }
}
